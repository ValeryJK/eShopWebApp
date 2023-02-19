using System.Text;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using BlazorShared;
using Microsoft.Extensions.Options;
using Microsoft.eShopWeb.ApplicationCore.Models.OrderReserved;

namespace Microsoft.eShopWeb.Web.Pages.Basket;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly IBasketService _basketService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOrderService _orderService;
    private string? _username = null;
    private readonly IBasketViewModelService _basketViewModelService;
    private readonly IAppLogger<CheckoutModel> _logger;
    private readonly IOrderReservationMessagingService _orderReservationMessagingService;

    private readonly HttpClient _httpClient;
    private readonly string _apiOrdersDeliveryBackgroundUrl; 
    private readonly string _apiOrdersDeliveryBackgroundKey;

    public CheckoutModel(IBasketService basketService,
        IOptions<BaseUrlConfiguration> baseUrlConfiguration,
        HttpClient httpClient,
        IBasketViewModelService basketViewModelService,
        SignInManager<ApplicationUser> signInManager,
        IOrderService orderService,
        IAppLogger<CheckoutModel> logger,
        IOrderReservationMessagingService orderReservationMessagingService
        )
    {
        _basketService = basketService;
        _signInManager = signInManager;
        _orderService = orderService;
        _basketViewModelService = basketViewModelService;
        _logger = logger;
        _httpClient = httpClient;

        _apiOrdersDeliveryBackgroundUrl = baseUrlConfiguration.Value.ApiOrdersDeliveryBackgroundUrl;      
        _apiOrdersDeliveryBackgroundKey = baseUrlConfiguration.Value.ApiOrdersDeliveryBackgroundKey;

        _orderReservationMessagingService = orderReservationMessagingService;
    }

    public BasketViewModel BasketModel { get; set; } = new BasketViewModel();

    public async Task OnGet()
    {
        await SetBasketModelAsync();
    }

    public async Task<IActionResult> OnPost(IEnumerable<BasketItemViewModel> items)
    {
        try
        {
            await SetBasketModelAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var updateModel = items.ToDictionary(b => b.Id.ToString(), b => b.Quantity);
            await _basketService.SetQuantities(BasketModel.Id, updateModel);
            var order = await _orderService.CreateOrderAsync(BasketModel.Id, new Address("123 Main St.", "Kent", "OH", "United States", "44240"));

            //Send order to Azure Function Delivery Service
            await SendOrdersBackgroundsApi(_apiOrdersDeliveryBackgroundUrl, ConvertToOrderDelivery(order), 
                _apiOrdersDeliveryBackgroundKey);

            var reservedOrders = order.OrderItems.Select(x => new OrderReserved { ItemId = x.ItemOrdered.CatalogItemId.ToString(), Quantity = x.Units }).ToArray();
            await _orderReservationMessagingService.PublishNewOrderMessageAsync(new OrderReservation { OrderReserved = reservedOrders });

            //Send order to Azure Service Bus
            await _basketService.DeleteBasketAsync(BasketModel.Id);
        }
        catch (EmptyBasketOnCheckoutException emptyBasketOnCheckoutException)
        {
            //Redirect to Empty Basket page
            _logger.LogWarning(emptyBasketOnCheckoutException.Message);
            return RedirectToPage("/Basket/Index");
        }

        return RedirectToPage("Success");
    }

    #region http request

    private async Task<bool> SendOrdersBackgroundsApi(string url, object content, string apiBackgroundKey,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var httpContent = CreateHttpContent(content, apiBackgroundKey))
            {
                request.Content = httpContent;

                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }

    }
    
    private HttpContent? CreateHttpContent(object content, string apiBackgroundKey)
    {
        HttpContent? httpContent = default;
        if (content != null)
        {
            var ms = new MemoryStream();
            SerializeJsonIntoStream(content, ms);
            ms.Seek(0, SeekOrigin.Begin);

            httpContent = new StreamContent(ms);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpContent.Headers.Add("x-functions-key", apiBackgroundKey);
        }

        return httpContent;

        void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
    }

    #endregion

    #region Convert

    private static OrderDelivery ConvertToOrderDelivery(ApplicationCore.Entities.OrderAggregate.Order order)
    {
        return new OrderDelivery
        {
            Id = order.Id.ToString(),
            Customer = order.BuyerId,
            Products = order.OrderItems.Select(x => new Product
            {
                Name = x.ItemOrdered.ProductName,
                Price = x.UnitPrice.ToString(),
                Units = x.Units.ToString()
            }).ToArray(),
            Address = order.ShipToAddress,
            TotalPrice = order.OrderItems.Sum(x => x.UnitPrice * x.Units)
        };
    }


    #endregion

    private async Task SetBasketModelAsync()
    {
        Guard.Against.Null(User?.Identity?.Name, nameof(User.Identity.Name));
        if (_signInManager.IsSignedIn(HttpContext.User))
        {
            BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
        }
        else
        {
            GetOrSetBasketCookieAndUserName();
            BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username!);
        }
    }

    private void GetOrSetBasketCookieAndUserName()
    {
        if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
        {
            _username = Request.Cookies[Constants.BASKET_COOKIENAME];
        }
        if (_username != null) return;

        _username = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions();
        cookieOptions.Expires = DateTime.Today.AddYears(10);
        Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
    }

}
