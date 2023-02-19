using System;
using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderItemsReserver.Configuration;
using OrderItemsReserver.Extensions;

[assembly: FunctionsStartup(typeof(OrderItemsReserver.Startup))]
namespace OrderItemsReserver;
internal class Startup : FunctionsStartup
{
    private IConfiguration _configuration;

    public override void Configure(IFunctionsHostBuilder builder)
    {
        ConfigureSettings();
       
        builder.Services.AddAppConfiguration(_configuration);
        builder.Services.AddStorageServices();

        builder.Services.Configure<LogicAppSettings>(options =>
           _configuration.GetSection("LogicAppSettings").Bind(options));
    }

    private void ConfigureSettings()
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(Environment.CurrentDirectory)
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddAzureKeyVault(new Uri($"https://eshop-keyvault-2023.vault.azure.net/"), new DefaultAzureCredential())
           .AddEnvironmentVariables()
           .Build();

        _configuration = config;

    }
}
