using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.MoveCouriers
{
    public class MoveCouriersHandler : IRequestHandler<MoveCouriersCommand, bool>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MoveCouriersHandler(IUnitOfWork unitOfWork,
                                   IOrderRepository orderRepository,
                                   ICourierRepository courierRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        public async Task<bool> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
        {
            var assignedOrders = (await _orderRepository.GetAssignedOrdersAsync()).ToList();
            if (!assignedOrders.Any())
                return false;

            foreach (var order in assignedOrders)
            {
                if (order.CourierId is null)
                {
                    return false;
                }
                var courier = await _courierRepository.GetCourierAsync((Guid)order.CourierId);
                if (courier is null)
                {
                    return false;
                }
                var moveResult = courier.Move(order.Location);
                if (moveResult.IsFailure)
                {
                    return false;
                }
                if (order.Location == courier.Location)
                {
                    order.CompleteOrder();
                    courier.SetFree();
                }
                _courierRepository.UpdateCourier(courier);
                _orderRepository.UpdateOrder(order);
            }
            return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
