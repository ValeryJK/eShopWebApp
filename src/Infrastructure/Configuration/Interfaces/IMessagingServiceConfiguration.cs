using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Configuration.Interfaces;
public interface IMessagingServiceConfiguration
{
    string ListenAndSendConnectionString { get; set; }
    string QueueName { get; set; }
}
