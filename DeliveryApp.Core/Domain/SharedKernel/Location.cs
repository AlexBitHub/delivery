using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public class Location : ValueObject
    {
        /// <summary>
        /// Минимально возможное значение координаты
        /// </summary>
        public const int MinValue = 2;

        /// <summary>
        /// Максимально возможное значение координаты
        /// </summary>
        public const int MaxValue = 10;

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

        /// <summary>
        /// Создать координату
        /// </summary>
        /// <param name="x">Значение по горизонтали</param>
        /// <param name="y">Значение по вертикали</param>
        /// <returns></returns>
        public static Result<Location, Error> Create(int x, int y)
        {
            if (x < MinValue || x > MaxValue) 
            {
                return new Error("coordinate.is.invalid", "Значение координаты X должно быть " +
                                                          "не меньше 1.1 и не больше 10.10.");
            }
            if (y < MinValue || y > MaxValue)
            {
                return new Error("coordinate.is.invalid", "Значение координаты Y должно быть " +
                                                          "не меньше 1.1 и не больше 10.10.");
            }
            return new Location(x, y);
        }

        /// <summary>
        /// Создать рандомную координату
        /// </summary>
        /// <returns></returns>
        public static Result<Location, Error> CreateRandomLocation()
        {
            var random = new Random();
            int x = random.Next(MinValue, MaxValue);
            int y = random.Next(MinValue, MaxValue);
            return Create(x, y).Value;
        }

        /// <summary>
        /// Расчет дистанции в шагах
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
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
            if (value < MinValue || value > MaxValue)
                return false;
            return true;
        }
    }
}
