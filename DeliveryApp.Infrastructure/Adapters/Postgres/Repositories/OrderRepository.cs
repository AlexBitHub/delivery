using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddOrderAsync(Order order)
        {
            if (order.Location != null) 
                _dbContext.Attach(order.Location);

            await _dbContext.Orders.AddAsync(order);           
        }

        public IEnumerable<Order> GetAssignedOrders()
        {
            var assignedOrders = _dbContext.Orders.Include(x => x.Location)
                                                  .Include(x => x.Status)
                                                  .Where(x => x.Status == OrderStatus.Assigned);

            return assignedOrders;
        }

        public IEnumerable<Order> GetNewOrders()
        {
            var assignedOrders = _dbContext.Orders.Include(x => x.Location)
                                                  .Include(x => x.Status)
                                                  .Where(x => x.Status == OrderStatus.Created);
            
            return assignedOrders;
        }

        public async Task<Order> GetOrder(Guid orderId)
        {
            var order = await _dbContext.Orders.Include(x => x.Location)
                                               .Include(x => x.Status)
                                               .FirstOrDefaultAsync(x => x.Id == orderId);

            return order;
        }

        public void UpdateOrder(Order order)
        {
            if (order.Location != null)
                _dbContext.Attach(order.Location);
            if (order.Status != null)
                _dbContext.Attach(order.Status);

            _dbContext.Orders.Update(order);
        }
    }
}
