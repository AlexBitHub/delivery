using DeliveryApp.Core.Domain.CourierAggregate;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public class TransportShould
    {
        [Theory]
        [MemberData(nameof(GetCorrectTestData))]
        public void CanBeFoundByName(int id, string name)
        {
            // Arrange 

            // Act
            var transport = Transport.FromName(name);

            // Assert
            transport.IsSuccess.Should().BeTrue();
            transport.Value.Name.Should().Be(name);
            transport.Value.Id.Should().Be(id);
        }

        [Theory]
        [MemberData(nameof(GetCorrectTestData))]
        public void CanBeFoundById(int id, string name)
        {
            // Arrange

            // Act
            var transport = Transport.FromId(id);

            // Assert
            transport.IsSuccess.Should().BeTrue();
            transport.Value.Id.Should().Be(id);
            transport.Value.Name.Should().Be(name);
        }

        [Theory]
        [MemberData(nameof(GetCorrectTestData))]
        public void BeEqualToTransportWithTheSameId(int id, string name)
        {
            // Arrange
            var transport1 = Transport.FromId(id);
            var transport2 = Transport.FromName(name);

            // Act
            var result = transport1.Equals(transport2);

            // Assert
            result.Should().BeTrue();

        }

        [Fact]
        public void ReturnTypesNamesList()
        {
            // Act
            var namesList = Transport.List();

            // Assert
            namesList.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("zebra")]
        [InlineData("helicopter")]
        [InlineData("rocket")]
        public void ReturnErrorWhenTransportNotFoundByName(string name)
        {
            // Act
            var transport = Transport.FromName(name);

            // Assert
            transport.IsSuccess.Should().BeFalse();
        }

        [Theory]
        [InlineData(123456789)]
        [InlineData(100000)]
        [InlineData(-1)]
        public void ReturnErrorWhenTransportNotFoundById(int id)
        {
            // Act
            var transport = Transport.FromId(id);

            // Assert
            transport.IsSuccess.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetCorrectTestData()
        {
            return new List<object[]>
            {
                new object[] { 1, "pedestrian" },
                new object[] { 2, "bicycle" },
                new object[] { 3, "car" }
            };
        }
    }
}
