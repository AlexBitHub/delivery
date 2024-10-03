using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchService : IDispatchService
    {
        public Courier Dispatch(Order order, List<Courier> couriers)
        {
            Courier fastestCourier = couriers.FirstOrDefault();
            double fastestTime = double.MaxValue;

            for (int i = 0; i < couriers.Count; i++)
            {
                var timeToOrder = couriers[i].CalculateTimeToDestionation(order.Location).Value;

                if (timeToOrder < fastestTime)
                {
                    fastestCourier = couriers[i];
                    fastestTime = timeToOrder;
                }
            }
            return fastestCourier;
        }
    }
}
