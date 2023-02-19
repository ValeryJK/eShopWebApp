using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Models.OrderReserved;
public class OrderDelivery
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    public string Customer { get; set; }
    public Product[]? Products { get; set; }
    public Address? Address { get; set; }
    public decimal TotalPrice { get; set; }
}
