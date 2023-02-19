using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Infrastructure.Models;

namespace Microsoft.eShopWeb.Infrastructure.Messages;
public class OrderReservationIntegrationMessage : IntegrationMessage
{
    public OrderReservedMessage[]? OrderReserved { get; set; }
}

public class OrderReservedMessage
{
    [JsonPropertyName("itemId")]
    public string ItemId { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
