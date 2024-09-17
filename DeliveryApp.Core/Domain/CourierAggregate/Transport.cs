using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    public class Transport : Entity<int>
    {
        public static readonly Transport Pedestrian = new Transport(1, nameof(Pedestrian).ToLowerInvariant(), 1);
        public static readonly Transport Bicycle = new Transport(2, nameof(Bicycle).ToLowerInvariant(), 2);
        public static readonly Transport Car = new Transport(3, nameof(Car).ToLowerInvariant(), 3);

        private Transport() { }

        private Transport(int id, string name, int speed) : this()
        {
            Id = id;
            Name = name;
            Speed = speed;
        }

        /// <summary>
        /// Название транспорта
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Скорость транспорта
        /// </summary>
        public int Speed { get; }
        

        /// <summary>
        /// Получить список видов транспорта
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Transport> List()
        {
            yield return Pedestrian;
            yield return Bicycle;
            yield return Car;
        }

        /// <summary>
        /// Получить транспорт по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Result<Transport, Error> FromName(string name) 
        {
            var transport = List().SingleOrDefault(t => string.Equals(t.Name,
                                                                      name,
                                                                      StringComparison.CurrentCultureIgnoreCase));
            if (transport == null) 
            {
                return Errors.StatusIsWrong();
            }
            return transport;
        }

        /// <summary>
        /// Получить транспорт по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Result<Transport, Error> FromId(int id)
        {
            var transport = List().SingleOrDefault(t => t.Id == id);
            if (transport == null)
            {
                return Errors.StatusIsWrong();
            }
            return transport;
        }

        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                string message = $"Неверное значение. " +
                                 $"Допустимые значения: {nameof(Transport).ToLowerInvariant()}: " +
                                 $"{string.Join(",", List().Select(s => s.Name))}";

                return new Error($"{nameof(Transport).ToLowerInvariant()}.is.wrong",
                                 message);
            }
        }

    }
}
