using DeliveryOrderService.Configuration.Interfaces;
using DeliveryOrderService.Data.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace DeliveryOrderService.Data;
public class OrderRepository : CosmosDbDataRepository<OrderDelivery>
{
    public OrderRepository(ICosmosDbConfiguration cosmosDbConfiguration, CosmosClient client,
        ILogger<OrderRepository> logger) : base(cosmosDbConfiguration, client, logger) { }

    public override string ContainerName => _cosmosDbConfiguration.OrderContainerName;
}
