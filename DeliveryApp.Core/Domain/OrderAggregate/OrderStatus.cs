using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    public class OrderStatus : Entity<int>
    {
        public static readonly OrderStatus Created = new OrderStatus(1, nameof(Created).ToLowerInvariant());
        public static readonly OrderStatus Assigned = new OrderStatus(2, nameof(Assigned).ToLowerInvariant());
        public static readonly OrderStatus Completed = new OrderStatus(3, nameof(Completed).ToLowerInvariant());
        private OrderStatus() { }

        private OrderStatus(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Название статуса заказа
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Список всех значений статусов
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<OrderStatus> List()
        {
            yield return Created;
            yield return Assigned;
            yield return Completed;
        }

        /// <summary>
        /// Получить статус по названию
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Result<OrderStatus, Error> FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, 
                                                                  name, 
                                                                  StringComparison.CurrentCultureIgnoreCase));
            if (state is null) 
            { 
                return Errors.StatusIsWrong(); 
            }
            return state;
        }

        /// <summary>
        /// Получить статус по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Result<OrderStatus, Error> FromId(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state is null)
            {
                return Errors.StatusIsWrong();
            }
            return state;
        }

        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                string message = $"Неверное значение. " +
                                 $"Допустимые значения: {nameof(OrderStatus).ToLowerInvariant()}: " +
                                 $"{string.Join(",", List().Select(s => s.Name))}";

                return new Error($"{nameof(OrderStatus).ToLowerInvariant()}.is.wrong",
                                 message);
            }
        }
    }
}
