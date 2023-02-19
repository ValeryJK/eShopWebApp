using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Models.OrderReserved;
public class OrderReservation : BaseEntity
{
    public OrderReserved[]? OrderReserved { get; set; }
}

public class OrderReserved
{
    [JsonPropertyName("itemId")]
    public string ItemId { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
