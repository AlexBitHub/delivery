using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application
{
    public class CreateOrderCommandShould
    {
        private readonly IOrderRepository _orderRepositoryMock;
        private readonly IUnitOfWork _unitOfWorkMock;

        public CreateOrderCommandShould()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _orderRepositoryMock = Substitute.For<IOrderRepository>();
        }

        [Fact]
        public async Task CreateNewOrder()
        {
            // Arrange            
            var orderId = Guid.NewGuid();

            _unitOfWorkMock.SaveEntitiesAsync()
                           .Returns(Task.FromResult(true));
            _orderRepositoryMock.AddOrderAsync(Arg.Any<Order>())
                                .Returns(Task.CompletedTask);

            var street = "Grove street";
            var command = new CreateOrderCommand(orderId, street);
            var handler = new CreateOrderHandler(_unitOfWorkMock, _orderRepositoryMock);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _orderRepositoryMock.Received(1);
        }
    }
}
