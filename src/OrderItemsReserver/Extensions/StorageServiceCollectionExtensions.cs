using Microsoft.Extensions.DependencyInjection;
using OrderItemsReserver.Configuration.Interfaces;
using OrderItemsReserver.Services.Storage.Interfaces;
using OrderItemsReserver.Services.Storage;
using Azure.Storage.Blobs;

namespace OrderItemsReserver.Extensions;
public static class StorageServiceCollectionExtensions
{
    public static IServiceCollection AddStorageServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var storageConfiguration = serviceProvider.GetRequiredService<IBlobStorageServiceConfiguration>();

        services.AddSingleton(factory => new BlobServiceClient(storageConfiguration.ConnectionPrimaryString));
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }
}
