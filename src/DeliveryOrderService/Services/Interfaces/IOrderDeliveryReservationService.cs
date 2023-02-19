using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrderService.Services.Interfaces;
public interface IOrderDeliveryReservationService
{
    Task HandleNewOrderReservationAsync(string orderDeliveryReservation);
}
