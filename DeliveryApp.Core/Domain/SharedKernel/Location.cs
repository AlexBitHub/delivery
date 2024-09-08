using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public class Location : ValueObject
    {
        [ExcludeFromCodeCoverage]
        private Location()
        {
        }

        [ExcludeFromCodeCoverage]
        private Location(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public static Result<Location, Error> Create(int x, int y)
        {
            var validationResult = ValidateCoordinate(x) && ValidateCoordinate(y);
            if (!validationResult)
            {
                var error = new Error("coordinate.is.invalid", "Значение координаты должно быть " +
                                                                         "не меньше 1.1 и не больше 10.10.");
                return error;
            }
            return new Location(x, y);
        }

        public static Result<Location, Error> CreateRandomLocation()
        {
            var random = new Random();
            int x = random.Next(2, 10);
            int y = random.Next(2, 10);
            return Create(x, y).Value;
        }

        public static int operator-(Location left, Location right)
        {
            var xDifference = Math.Abs(left.X - right.X);
            var yDifference = Math.Abs(left.Y - right.Y);
            return xDifference + yDifference;
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }

        private static bool ValidateCoordinate(int value)
        {
            if (value < 1.1 || value > 10.10)
                return false;
            return true;
        }
    }
}
