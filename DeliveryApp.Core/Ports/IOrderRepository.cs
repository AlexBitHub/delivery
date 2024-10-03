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
        Task<IEnumerable<Order>> GetNewOrdersAsync();

        /// <summary>
        /// Заказы со статусом Assigned
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetAssignedOrdersAsync();
    }
}
