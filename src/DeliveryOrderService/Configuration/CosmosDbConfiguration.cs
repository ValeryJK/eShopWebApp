using DeliveryOrderService.Configuration.Interfaces;
using Microsoft.Extensions.Options;

namespace DeliveryOrderService.Configuration;
public class CosmosDbConfiguration : ICosmosDbConfiguration
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string OrderContainerName { get; set; }
    public string OrderContainerPartitionKeyPath { get; set; }
}

public class CosmosDbConfigurationValidation : IValidateOptions<CosmosDbConfiguration>
{
    public ValidateOptionsResult Validate(string name, CosmosDbConfiguration options)
    {
        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ConnectionString)} configuration parameter for the Azure Cosmos DB is required");
        }

        if (string.IsNullOrEmpty(options.OrderContainerName))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.OrderContainerName)} configuration parameter for the Azure Cosmos DB is required");
        }

        if (string.IsNullOrEmpty(options.DatabaseName))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.DatabaseName)} configuration parameter for the Azure Cosmos DB is required");
        }

        if (string.IsNullOrEmpty(options.OrderContainerPartitionKeyPath))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.OrderContainerPartitionKeyPath)} configuration parameter for the Azure Cosmos DB is required");
        }

        return ValidateOptionsResult.Success;
    }
}
