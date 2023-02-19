using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Models;
public abstract class IntegrationMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
