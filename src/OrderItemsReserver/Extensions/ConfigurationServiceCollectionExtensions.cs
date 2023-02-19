using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderItemsReserver.Configuration.Interfaces;
using OrderItemsReserver.Configuration;

namespace OrderItemsReserver.Extensions;
public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<BlobStorageServiceConfiguration>(config.GetSection("BlobStorageSettings"));
        services.AddSingleton<IValidateOptions<BlobStorageServiceConfiguration>, BlobStorageServiceConfigurationValidation>();
        var blobStorageServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<BlobStorageServiceConfiguration>>().Value;
        services.AddSingleton<IBlobStorageServiceConfiguration>(blobStorageServiceConfiguration);
               
        return services;
    }
}
