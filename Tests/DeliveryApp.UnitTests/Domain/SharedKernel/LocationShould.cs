using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;


namespace DeliveryApp.UnitTests.Domain.SharedKernel
{
    public class LocationShould
    {
        [Theory]
        [InlineData(2, 3)]
        [InlineData(4, 5)]
        [InlineData(6, 7)]
        [InlineData(8, 9)]
        [InlineData(10, 9)]
        public void BeCorrectWhenCoordiantesAreCorrectOnCreate(int x, int y)
        {
            var location = Location.Create(x, y);

            location.IsSuccess.Should().BeTrue();

            location.Value.X.Equals(x);
            location.Value.Y.Equals(y);
        }

        [Theory]
        [InlineData(-1, 5)]
        [InlineData(1, 11)]
        [InlineData(20, 11)]
        public void ReturnErrorWhenCoordinatesAreIncorrectOnCreate(int x, int y)
        {
            var location = Location.Create(x, y);

            location.IsFailure.Should().BeTrue();
        }

        [Theory]
        [InlineData(5, 8, 7, 2, 8)]
        [InlineData(2, 9, 3, 6, 4)]
        public void CalculateDistanceBetweenTwoInstance(int x1, int y1, int x2, int y2, int result)
        {
            var location1 = Location.Create(x1, y1).Value;
            var location2 = Location.Create(x2, y2).Value;

            var calcResult = location1 - location2;

            calcResult.Should().Be(result);
        }

        [Fact]
        public void BeEqualWhenAllPropertiesAreEqual()
        {
            var location1 = Location.Create(5, 4).Value;
            var location2 = Location.Create(5, 4).Value;

            var equalResult = location1 == location2;

            equalResult.Should().BeTrue();
        }

        [Fact]
        public void BeNotEqualWhenNotAllPropertiesAreEqual()
        {
            var location1 = Location.Create(5, 4).Value;
            var location2 = Location.Create(2, 7).Value;

            var equalResult = location1 == location2;

            equalResult.Should().BeFalse();
        }

        [Fact]
        public void CreateRandomLocation()
        {
            var randomLocation = Location.CreateRandomLocation();

            randomLocation.IsSuccess.Should().BeTrue();
            randomLocation.Value.X.Should().BeInRange(Location.MinValue, Location.MaxValue);
            randomLocation.Value.Y.Should().BeInRange(Location.MinValue, Location.MaxValue);
        }
    }
}
