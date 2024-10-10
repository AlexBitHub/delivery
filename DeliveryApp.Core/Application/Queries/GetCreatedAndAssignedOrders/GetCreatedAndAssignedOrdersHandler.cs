using Dapper;
using DeliveryApp.Core.Domain.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetOpenOrders
{
    public class GetCreatedAndAssignedOrdersHandler : IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>
    {
        private string _connectionString;

        public GetCreatedAndAssignedOrdersHandler(string connectionString) 
        {
            _connectionString = connectionString;
        }

        public async Task<GetCreatedAndAssignedOrdersResponse> Handle(GetCreatedAndAssignedOrdersQuery request,
                                                                      CancellationToken cancellationToken)
        {

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sqlQuery = $@"SELECT id, courier_id, location_x, location_y,
                            status_id FROM public.orders WHERE status_id!=@status_id;";
            var commandDefinition = new CommandDefinition(sqlQuery, 
                                                          new { status_id = OrderStatus.Completed },
                                                          cancellationToken: cancellationToken);
            var queryRes = await connection.QueryAsync(commandDefinition);
            if (queryRes.AsList().Count == 0)
            {
                return null;
            }

            var orders = new List<OrderMap>();
            foreach (var item in queryRes) 
            {
                orders.Add(MapToOrder(item));
            }

            return new GetCreatedAndAssignedOrdersResponse(orders);
        }

        private OrderMap MapToOrder(dynamic result)
        {
            return new OrderMap()
            {
                Id = result.id,
                Location = new LocationMap()
                {
                    X = result.location_x,
                    Y = result.location_y,
                }
            };
        }
    }
}
