using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class OrderRepositoryShould
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgre:14.7")
            .WithDatabase("order")
            .WithUsername("usrname")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();

        private ApplicationDbContext _dbContext;
        private Location _orderLocation;
        private Courier _courier;
        private Order[] _orders;

        public OrderRepositoryShould() 
        {
            var location = Location.Create(3, 8);
            location.IsSuccess.Should().BeTrue();
            _orderLocation = location.Value;

            var courier = Courier.Create("Володя",
                                         Transport.Bicycle,
                                         Location.CreateRandomLocation().Value);
            courier.IsSuccess.Should().BeTrue();
            _courier = courier.Value;

            _orders = new Order[3];
            _orders[0] = Order.Create(Guid.NewGuid(), Location.CreateRandomLocation().Value).Value;
            _orders[1] = Order.Create(Guid.NewGuid(), Location.CreateRandomLocation().Value).Value;
            var assignedOrder = Order.Create(Guid.NewGuid(), Location.CreateRandomLocation().Value).Value;
            var assignedResult = assignedOrder.AssignOnCourier(_courier);
            assignedResult.IsSuccess.Should().BeTrue();
            
        }

        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync().AsTask();
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            var contextOpts = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOpt => { sqlOpt.MigrationsAssembly("DeliveryApp.Infrastructure"); }).Options;
            _dbContext = new ApplicationDbContext(contextOpts);
            _dbContext.Database.Migrate();
        }

        public async Task AddOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, _orderLocation);
            order.IsSuccess.Should().BeTrue();

            // Act
            var orderRepos = new OrderRepository(_dbContext);
            await orderRepos.AddOrderAsync(order.Value);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            // Assert
            var orderFromDb = await orderRepos.GetOrder(orderId);
            orderFromDb.Should().BeEquivalentTo(order);
        }

        public async Task UpdateOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, _orderLocation);
            order.IsSuccess.Should().BeTrue();

            var orderRepos = new OrderRepository(_dbContext);
            await orderRepos.AddOrderAsync(order.Value);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            // Act 
            var assignResult = order.Value.AssignOnCourier(_courier);
            assignResult.IsSuccess.Should().BeTrue();
            orderRepos.UpdateOrder(order.Value);
            await unitOfWork.SaveEntitiesAsync();

            // Assert
            var orderFromDb = await orderRepos.GetOrder(orderId);
            orderFromDb.Should().BeEquivalentTo(order.Value);
        }

        public async Task GetOrderById()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, _orderLocation);
            order.IsSuccess.Should().BeTrue();

            // Act
            var orderRepos = new OrderRepository(_dbContext);
            await orderRepos.AddOrderAsync(order.Value);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            // Assert
            var orderFromDb = await orderRepos.GetOrder(orderId);
        }

        public async void GetNewOrders()
        {
            // Arrange
            var orderRepos = new OrderRepository(_dbContext);
            foreach (var order in _orders)
            {
                await orderRepos.AddOrderAsync(order);
            }
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            var newTestOrders = _orders.Where(x => x.Status == OrderStatus.Created).ToArray();
            
            // Act
            var newOrdersFromDb = orderRepos.GetNewOrders().ToArray();

            // Assert
            newOrdersFromDb.Should().BeEquivalentTo(newOrdersFromDb);
        }

        public async Task GetAssignedOrders()
        {
            // Arrange
            var orderRepos = new OrderRepository( _dbContext);
            foreach (var order in _orders)
            {
                await orderRepos.AddOrderAsync(order);
            }
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            var assignedTestOrders = _orders.Where(x => x.Status == OrderStatus.Assigned).ToArray();

            // Act
            var assignedOrdersFromDb = orderRepos.GetAssignedOrders().ToArray();

            // Assert
            assignedOrdersFromDb.Should().BeEquivalentTo(assignedTestOrders);
        }
    }
}
