using DeliveryOrderService.Configuration.Interfaces;
using DeliveryOrderService.Data;
using DeliveryOrderService.Data.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryOrderService.Configuration;
public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var cosmoDbConfiguration = serviceProvider.GetRequiredService<ICosmosDbConfiguration>();
        CosmosClient cosmosClient = new CosmosClient(cosmoDbConfiguration.ConnectionString);
        Database database = cosmosClient.CreateDatabaseIfNotExistsAsync(cosmoDbConfiguration.DatabaseName).GetAwaiter().GetResult();

        database.CreateContainerIfNotExistsAsync(cosmoDbConfiguration.OrderContainerName, 
            cosmoDbConfiguration.OrderContainerPartitionKeyPath, 400).GetAwaiter().GetResult();

        services.AddSingleton(cosmosClient);
        services.AddSingleton<IDataRepository<OrderDelivery>, OrderRepository>();

        return services;
    }
}
