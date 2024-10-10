using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchService : IDispatchService
    {
        public Courier Dispatch(Order order, List<Courier> couriers)
        {
            if (order is null) throw new ArgumentNullException(nameof(order));
            if (couriers is null || couriers.Count == 0) throw new ArgumentNullException(nameof(couriers));

            Courier fastestCourier = couriers.FirstOrDefault();
            double fastestTime = double.MaxValue;

            for (int i = 0; i < couriers.Count; i++)
            {
                var timeToOrderRes = couriers[i].CalculateTimeToDestionation(order.Location);
                if (timeToOrderRes.IsFailure)
                {
                    continue;
                }

                var timeToOrder = timeToOrderRes.Value;

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
