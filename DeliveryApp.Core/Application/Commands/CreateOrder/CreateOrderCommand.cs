using MediatR;

namespace DeliveryApp.Core.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<bool>
    {
        public CreateOrderCommand(Guid orderId, string street) 
        {
            if (orderId == Guid.Empty) throw new ArgumentNullException(nameof(orderId));
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentNullException(nameof(street));

            BasketId = orderId;
            Street = street;
        }

        /// <summary>
        /// Идентификатор корзины
        /// </summary>
        /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
        public Guid BasketId { get; }

        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; }
    }
}
