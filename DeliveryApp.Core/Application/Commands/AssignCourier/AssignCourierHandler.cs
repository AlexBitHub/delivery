using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.AssignCourier
{
    public class AssignOrdersHandler : IRequestHandler<AssignCourierCommand, bool>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IDispatchService _dispatchService;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;


        public AssignOrdersHandler(IUnitOfWork unitOfWork,
                                   IOrderRepository orderRepository,
                                   ICourierRepository courierRepository,
                                   IDispatchService dispatchService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _dispatchService = dispatchService ?? throw new ArgumentNullException(nameof(dispatchService));
        }

        public async Task<bool> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
        {
            var order = (await _orderRepository.GetAssignedOrdersAsync())?.FirstOrDefault();
            if (order == null) 
            {
                return false;
            }
            var couriers = (await _courierRepository.GetFreeCouriersAsync())?.ToList();
            if (couriers is null || !couriers.Any()) 
            {
                return false;
            }

            var dispatchCourier = _dispatchService.Dispatch(order, couriers);
            if (dispatchCourier is null) 
                return false;
            
            // Назначаем заказ на курьера
            var orderAssignToCourierResult = order.AssignOnCourier(dispatchCourier);
            if (orderAssignToCourierResult.IsFailure) 
                return false;

            // Делаем курьера занятым
            var courierSetBusyResult = dispatchCourier.SetBusy();
            if (courierSetBusyResult.IsFailure) 
                return false;

            // Сохраняем
            _courierRepository.UpdateCourier(dispatchCourier);
            _orderRepository.UpdateOrder(order);
            return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
