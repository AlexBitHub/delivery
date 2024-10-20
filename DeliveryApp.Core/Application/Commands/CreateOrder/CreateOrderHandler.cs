using System.Net.Http.Headers;
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
        private IGeoClient _geoClient;

        public CreateOrderHandler(IUnitOfWork unitWork, 
                                  IOrderRepository orderRepository,
                                  IGeoClient geoClient) 
        {
            _orderRepository = orderRepository ?? throw new ArgumentException(nameof(orderRepository));
            _unitOfWork = unitWork ?? throw new ArgumentException(nameof(unitWork));
            _geoClient = geoClient ?? throw new ArgumentException(nameof(geoClient));
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {            
            // TODO: получать Location из сервиса Geo
            var location = await _geoClient.GetGeolocationAsync(request.Street, cancellationToken);
            var order = Order.Create(request.BasketId, location.Value);
            if (order.IsFailure)
                return false;
                
            await _orderRepository.AddOrderAsync(order.Value);
                        
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);
            return true;
        }
    }
}
