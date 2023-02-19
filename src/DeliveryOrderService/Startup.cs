using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using DeliveryOrderService.Configuration;
using DeliveryOrderService.Services;
using DeliveryOrderService.Services.Interfaces;

[assembly: FunctionsStartup(typeof(DeliveryOrderService.Startup))]
namespace DeliveryOrderService;
internal class Startup : FunctionsStartup
{
    private IConfiguration _configuration;

    public override void Configure(IFunctionsHostBuilder builder)
    {
        ConfigureSettings();

        builder.Services.AddAppConfiguration(_configuration);        
        builder.Services.AddDataServices();

        builder.Services.AddScoped<IOrderDeliveryReservationService, OrderDeliveryReservationService>();
    }

    private void ConfigureSettings()
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(Environment.CurrentDirectory)
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddAzureKeyVault(new Uri($"https://eshop-keyvault-2023.vault.azure.net/"), 
                new DefaultAzureCredential())
           .AddEnvironmentVariables()
           .Build();

        _configuration = config;
    }
}
