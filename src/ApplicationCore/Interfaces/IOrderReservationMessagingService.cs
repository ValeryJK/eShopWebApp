using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Models.OrderReserved;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;
public interface IOrderReservationMessagingService
{
    Task<bool> PublishNewOrderMessageAsync(OrderReservation orderReservation);
}
