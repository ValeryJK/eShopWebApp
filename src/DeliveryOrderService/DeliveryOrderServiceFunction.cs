using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DeliveryOrderService.Services.Interfaces;

namespace DeliveryOrderService
{
    public class DeliveryOrderServiceFunction
    {
        private readonly IOrderDeliveryReservationService _service;

        public DeliveryOrderServiceFunction(IOrderDeliveryReservationService service)
        {
            _service = service;
        }

        [FunctionName("DeliveryOrderServiceFunction")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post",
            Route = "deliveryOrders")] HttpRequest req, ILogger logger)
        {
            try
            {
                logger.LogInformation("C# HTTP trigger function processed order to Cosmos DB.");
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBody))
                {
                    logger.LogInformation("Error: body is empty ...");
                    return new ObjectResult(false);
                }

                await _service.HandleNewOrderReservationAsync(requestBody);
                return new OkObjectResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error created new order ... {ex.Message}");
                return new ObjectResult(false);
            }
        }
    }
}
