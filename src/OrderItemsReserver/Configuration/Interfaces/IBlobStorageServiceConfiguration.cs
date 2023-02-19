using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderItemsReserver.Configuration.Interfaces;
public interface IBlobStorageServiceConfiguration
{
    string ContainerName { get; set; }
    string ConnectionPrimaryString { get; set; }
    string Key { get; set; }
    string AccountName { get; set; }
}
