using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public class CourierShould
    {
        [Fact]
        public void BeCorrectWhenAllParamsCorrect()
        {
            // Arrange
            var location = Location.CreateRandomLocation().Value;

            // Act
            var courier = Courier.Create("courier", Transport.Bicycle, location);

            // Assert
            courier.IsSuccess.Should().BeTrue();
            courier.Value.Location.Should().Be(location);
            courier.Value.Status.Should().Be(CourierStatus.Free);
        }

        [Fact]
        public void ReturnErrorWhenNameIsEmpty()
        {
            // Arrange
            var location = Location.CreateRandomLocation().Value;

            // Act
            var courier = Courier.Create("", Transport.Bicycle, location);

            // Assert
            courier.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeBusyWhenIsNotBusy()
        {
            // Arrange
            var location = Location.CreateRandomLocation().Value;
            var courier = Courier.Create("courier", Transport.Bicycle, location);

            // Act
            var setBusyResult = courier.Value.SetBusy();

            // Assert
            setBusyResult.IsSuccess.Should().BeTrue();
            courier.Value.Status.Should().Be(CourierStatus.Busy);
        }


        [Fact]
        public void ReturnErrorWhenSetBusyBusyCourier()
        {
            // Arrange
            var location = Location.CreateRandomLocation().Value;
            var courier = Courier.Create("courier", Transport.Bicycle, location);
            courier.Value.SetBusy();

            // Act
            var setBusyResult = courier.Value.SetBusy();

            // Assert
            setBusyResult.IsFailure.Should().BeTrue();
            courier.Value.Status.Should().Be(CourierStatus.Busy);
        }

        [Fact]
        public void BeFreeWhenIsNotFree()
        {
            // Arrange
            var location = Location.CreateRandomLocation().Value;
            var courier = Courier.Create("courier", Transport.Bicycle, location);
            courier.Value.SetBusy();

            // Act 
            var setFreeResult = courier.Value.SetFree();

            // Assert
            setFreeResult.IsSuccess.Should().BeTrue();
            courier.Value.Status.Should().Be(CourierStatus.Free);
        }

        [Fact]
        public void ReturnErrorWhenSetFreeFreeCourier()
        {
            // Arrange
            var location = Location.CreateRandomLocation().Value;
            var freeCourier = Courier.Create("courier", Transport.Bicycle, location);

            // Act
            var setFreeResult = freeCourier.Value.SetFree();

            // Assert
            setFreeResult.IsFailure.Should().BeTrue();
            freeCourier.Value.Status.Should().Be(CourierStatus.Free);
        }

        [Theory]
        [MemberData(nameof(GetCouriersAndDestinationData))]
        public void CorrectCalculateTimeToDestination(Courier courier,
                                                      Location destination,
                                                      double timeRes)
        {
            // Arrange

            // Act
            var calcResult = courier.CalculateTimeToDestionation(destination);

            // Assert
            calcResult.IsSuccess.Should().BeTrue();
            calcResult.Value.Should().Be(timeRes);
        }

        public static IEnumerable<object[]> GetCouriersAndDestinationData()
        {
            return new List<object[]>
            {
                new object[] 
                { 
                    Courier.Create("courier",
                                   Transport.Pedestrian,
                                   Location.Create(2, 2).Value).Value,
                    Location.Create(3, 3).Value,
                    2
                },
                new object[]
                {
                    Courier.Create("courier",
                                   Transport.Bicycle,
                                   Location.Create(2, 4).Value).Value,
                    Location.Create(9, 8).Value,
                    5.5
                },
                new object[]
                {
                    Courier.Create("courier",
                                   Transport.Car,
                                   Location.Create(3, 3).Value).Value,
                    Location.Create(7, 8).Value,
                    3
                }
            };
        }
    }
}
