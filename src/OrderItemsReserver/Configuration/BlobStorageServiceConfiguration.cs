using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OrderItemsReserver.Configuration.Interfaces;

namespace OrderItemsReserver.Configuration;
public class BlobStorageServiceConfiguration : IBlobStorageServiceConfiguration
{
    public string ConnectionPrimaryString { get; set; }
    public string ContainerName { get; set; }    
    public string Key { get; set; }
    public string AccountName { get; set; }
}

public class BlobStorageServiceConfigurationValidation : IValidateOptions<BlobStorageServiceConfiguration>
{
    public ValidateOptionsResult Validate(string name, BlobStorageServiceConfiguration options)
    {

        if (string.IsNullOrEmpty(options.ConnectionPrimaryString))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ConnectionPrimaryString)} configuration parameter for the Azure Storage Account is required");
        }

        if (string.IsNullOrEmpty(options.ContainerName))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ContainerName)} configuration parameter for the Azure Storage Account is required");
        }

        if (string.IsNullOrEmpty(options.Key))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Key)} configuration parameter for the Azure Storage Account is required");
        }

        if (string.IsNullOrEmpty(options.AccountName))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.AccountName)} configuration parameter for the Azure Storage Account is required");
        }

        return ValidateOptionsResult.Success;
    }
}
