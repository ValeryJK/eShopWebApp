using Microsoft.Extensions.DependencyInjection.Extensions;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.eShopWeb.Infrastructure.Configuration.Interfaces;

namespace Microsoft.eShopWeb.Web.Extensions;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingServices(this IServiceCollection services)
    {
        var options = new ServiceBusClientOptions();
        options.RetryOptions = new ServiceBusRetryOptions
        {
            Delay = TimeSpan.FromSeconds(10),
            MaxDelay = TimeSpan.FromSeconds(30),
            Mode = ServiceBusRetryMode.Fixed,
            MaxRetries = 3,
        };

        services.TryAddSingleton(implementationFactory =>
        {
            var serviceBusConfiguration = implementationFactory.GetRequiredService<IMessagingServiceConfiguration>();
            var serviceBusClient = new ServiceBusClient(serviceBusConfiguration.ListenAndSendConnectionString, options);
            var serviceBusSender = serviceBusClient.CreateSender(serviceBusConfiguration.QueueName);
            return serviceBusSender;
        });

        services.AddSingleton<IOrderReservationMessagingService, OrderReservationMessagingService>();

        return services;
    }
}
