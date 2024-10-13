using System;
using Api.Controllers;
using Api.Models;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.Queries.GetOpenOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.Api.Adapters.Http;

public class DeliveryController : DefaultApiController
{
    private IMediator _mediator;

    public DeliveryController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> CreateOrder()
    {
        var id = Guid.NewGuid();
        var street = "Not existed street";
        var createCommand = new CreateOrderCommand(id, street);

        var response = await _mediator.Send(createCommand);
        if (response) 
            return Ok();
        return Conflict();
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var getCouriers = new GetBusyCouriersQuery();
        var response = await _mediator.Send(getCouriers);

        if (response is null)
            return NoContent();
        var couriers = response.Couriers.Select(x => new Courier()
        {
           Id = x.Id,
           Location = new Location()
           {
                X = x.Location.X,
                Y = x.Location.Y
           },
            Name = x.Name
        });
        return Ok(couriers);
    }

    public override async Task<IActionResult> GetOrders()
    {
        var getOrdersCommand = new GetCreatedAndAssignedOrdersQuery();
        var response = await _mediator.Send(getOrdersCommand);

        if (response is null)
            return NoContent();
        
        var orders = response.OrderMaps.Select(x => new Order()
        {
            Id = x.Id,
            Location = new Location()
            {
                X = x.Location.X,
                Y = x.Location.Y
            }
        });
        return Ok(orders);
    }
}
