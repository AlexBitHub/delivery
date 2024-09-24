using DeliveryApp.Core.Domain.OrderAggregate;

namespace DeliveryApp.Core.Ports
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        void UpdateOrder(Order order);
        Task<Order> GetOrder(Guid orderId);

        /// <summary>
        /// Заказы со статусом Created
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetNewOrders();

        /// <summary>
        /// Заказы со статусом Assigned
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetAssignedOrders();
    }
}
