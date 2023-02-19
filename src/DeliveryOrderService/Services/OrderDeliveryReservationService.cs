using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryOrderService.Data;
using DeliveryOrderService.Data.Entities;
using DeliveryOrderService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace DeliveryOrderService.Services;
public class OrderDeliveryReservationService : IOrderDeliveryReservationService
{    
    private readonly IDataRepository<OrderDelivery> _orderDeliveryRepository;
    private readonly ILogger<OrderDeliveryReservationService> _logger;

    public OrderDeliveryReservationService(IDataRepository<OrderDelivery> orderDeliveryRepository, 
        ILogger<OrderDeliveryReservationService> logger)
    {
        _orderDeliveryRepository = orderDeliveryRepository ?? 
            throw new ArgumentNullException(nameof(orderDeliveryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleNewOrderReservationAsync(string orderDeliveryReservation)
    {
        var schema = new JSchemaGenerator().Generate(typeof(OrderDelivery));
        var isValidSchema = JObject.Parse(orderDeliveryReservation).IsValid(schema, out IList<string> errors);
        var order = JsonConvert.DeserializeObject<OrderDelivery>(orderDeliveryReservation);

        await _orderDeliveryRepository.AddAsync(order);

        _logger.LogInformation("Order added to Azure Cosmos DB.");
    }
}
