using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private IOrderRepository _orderRepository;
        private IUnitOfWork _unitOfWork;

        public CreateOrderHandler(IUnitOfWork unitWork, IOrderRepository orderRepos) 
        {
            _orderRepository = orderRepos;
            _unitOfWork = unitWork;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {            
            // TODO: получать Location из сервиса Geo
            var location = Location.CreateRandomLocation();
            var order = Order.Create(request.BasketId, location.Value);

            await _orderRepository.AddOrderAsync(order.Value);
                        
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);
            return true;
        }
    }
}
