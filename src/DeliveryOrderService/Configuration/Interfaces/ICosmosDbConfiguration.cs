using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrderService.Configuration.Interfaces
{
    public interface ICosmosDbConfiguration
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string OrderContainerName { get; set; }
        string OrderContainerPartitionKeyPath { get; set; }
    }
}
