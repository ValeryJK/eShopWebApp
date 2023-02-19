using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using OrderItemsReserver.Services.Storage.Interfaces;
using OrderItemsReserver.Configuration;
using Microsoft.Extensions.Options;

namespace OrderItemsReserver
{
    public class OrderItemsReserverFunction
    {
        private static HttpClient _httpClient = new HttpClient();
        private readonly LogicAppSettings _config;
        private readonly IBlobStorageService _blobStorageService;

        public OrderItemsReserverFunction(IOptions<LogicAppSettings> options, IBlobStorageService blobStorageService)
        {
            _config = options?.Value ?? new LogicAppSettings();
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        }

        [FunctionName("OrderItemsReserverFunction")]
        public async Task RunAsync([ServiceBusTrigger("orderreserved", Connection = "eshopweb_RootManageSharedAccessKey_SERVICEBUS")] ServiceBusReceivedMessage messageItem,
            ServiceBusMessageActions messageActions, ILogger log)
        {
            try
            {
                var count = 3;
                var message = Encoding.UTF8.GetString(messageItem.Body);
                log.LogInformation($"C# ServiceBus queue trigger function processed message: {message}" +
                    $" - {messageItem.DeliveryCount}");
                
                if (messageItem.DeliveryCount > count)
                {
                    log.LogInformation($"Quantity exceeded: {messageItem.DeliveryCount}");

                    await messageActions.DeadLetterMessageAsync(messageItem);
                    await PostMessageAsync(_config.LogicApiUri, body: "Alarm!!!");

                    return;
                }

                await _blobStorageService.UploadBlobAsync(messageItem.Body.ToStream(), $"{Guid.NewGuid()}.json");  
                
                await messageActions.CompleteMessageAsync(messageItem);
            }
            catch (Exception ex)
            {
                log.LogWarning($"Error: {ex.Message}");
                await PostMessageAsync(_config.LogicApiUri, body: "Alarm!!!");

                await Task.Delay(10000);
                await messageActions.AbandonMessageAsync(messageItem);
            }

            async Task PostMessageAsync(string url, string body) =>
                await _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new { msg = body }), Encoding.UTF8, "application/json"));
        }
    }
}
