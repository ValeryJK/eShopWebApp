using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.Infrastructure.Messages;
using Microsoft.eShopWeb.ApplicationCore.Models.OrderReserved;

namespace Microsoft.eShopWeb.Infrastructure.Services;
public class OrderReservationMessagingService : IOrderReservationMessagingService
{
    private readonly ServiceBusSender _serviceBusSender;

    public OrderReservationMessagingService(ServiceBusSender serviceBusSender)
    {
        _serviceBusSender = serviceBusSender
            ?? throw new ArgumentNullException(nameof(serviceBusSender));
    }

    public async Task<bool> PublishNewOrderMessageAsync(OrderReservation orderReservation)
    {
        try
        {
            var orderReservationIntegrationMessage = new OrderReservationIntegrationMessage
            {
                Id = Guid.NewGuid().ToString(),
                OrderReserved = orderReservation.OrderReserved?.Select(x => new OrderReservedMessage { ItemId = x.ItemId, Quantity = x.Quantity }).ToArray()
            };

            var serializedMessage = JsonSerializer.Serialize(orderReservationIntegrationMessage);
            ServiceBusMessage message = new ServiceBusMessage(serializedMessage);
            await _serviceBusSender.SendMessageAsync(message);

            return true;

        }
        catch (Exception ex)
        {
            return false;
        }

    }
}
