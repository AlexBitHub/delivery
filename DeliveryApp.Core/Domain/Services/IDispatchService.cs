using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services
{
    public interface IDispatchService
    {
        Courier Dispatch(Order order, List<Courier> couriers);
    }
}
