using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.OrderAggregate
{
    public class OrderShould
    {
        [Fact]
        public void BeCorrectWhenGuidIsNotEmpty()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var location = Location.CreateRandomLocation().Value;

            // Act
            var order = Order.Create(guid, location);

            // Assert
            order.IsSuccess.Should().BeTrue();
            order.Value.Location.Should().Be(location);
        }

        [Fact]
        public void ReturnErrorWhenGuidIsEmpty()
        {
            // Arrange
            var guid = Guid.Empty;
            var location = Location.CreateRandomLocation().Value;

            // Act
            var order = Order.Create(guid, location);

            // Assert
            order.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeAssignedOnCourier() 
        {
            // Arrange 
            var orderLocation = Location.CreateRandomLocation().Value;
            var orderGuid = Guid.NewGuid();
            var order = Order.Create(orderGuid, orderLocation);

            var courierName = "courier";
            var courierTransport = Transport.Bicycle;
            var courierLocation = Location.CreateRandomLocation().Value;
            var courier = Courier.Create(courierName, courierTransport, courierLocation).Value;

            // Act
            var assignResult = order.Value.AssignOnCourier(courier);

            // Arrange
            assignResult.IsSuccess.Should().BeTrue();
            order.Value.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public void ReturnErrorWhenAssignBusyCourier()
        {
            // Arrange 
            var orderLocation = Location.CreateRandomLocation().Value;
            var orderGuid = Guid.NewGuid();
            var order = Order.Create(orderGuid, orderLocation);

            var courierName = "courier";
            var courierTransport = Transport.Bicycle;
            var courierLocation = Location.CreateRandomLocation().Value;
            var courier = Courier.Create(courierName, courierTransport, courierLocation).Value;
            courier.SetBusy();

            // Act
            var assignResult = order.Value.AssignOnCourier(courier);

            // Arrange
            assignResult.IsFailure.Should().BeTrue();
            order.Value.Status.Should().NotBe(OrderStatus.Assigned);
        }


        [Fact]
        public void BeCompletedWhenAssigned()
        {
            // Arrange
            var orderLocation = Location.CreateRandomLocation().Value;
            var orderGuid = Guid.NewGuid();
            var order = Order.Create(orderGuid, orderLocation);
            
            var courierName = "courier";
            var courierTransport = Transport.Bicycle;
            var courierLocation = Location.CreateRandomLocation().Value;
            var courier = Courier.Create(courierName, courierTransport, courierLocation).Value;
            order.Value.AssignOnCourier(courier);

            // Act
            var completeResult = order.Value.CompleteOrder();

            // Assert
            completeResult.IsSuccess.Should().BeTrue();
            order.Value.Status.Should().Be(OrderStatus.Completed);
        }
        
        [Fact]        
        
        public void ReturnErrorWhenCompleteNotAssigned()
        {
            // Arrange
            var orderLocation = Location.CreateRandomLocation().Value;
            var orderGuid = Guid.NewGuid();
            var order = Order.Create(orderGuid, orderLocation);

            // Act
            var completeResult = order.Value.CompleteOrder();

            // Assert
            completeResult.IsFailure.Should().BeTrue();
            order.Value.Status.Should().NotBe(OrderStatus.Completed);
        }
    }
}
