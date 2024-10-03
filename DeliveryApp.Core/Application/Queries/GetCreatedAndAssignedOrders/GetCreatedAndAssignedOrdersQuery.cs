using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetOpenOrders
{
    public class GetCreatedAndAssignedOrdersQuery : IRequest<GetCreatedAndAssignedOrdersResponse>
    {
        public GetCreatedAndAssignedOrdersQuery() { }
    }
}
