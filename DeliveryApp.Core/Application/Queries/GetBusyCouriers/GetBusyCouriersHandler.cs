using Dapper;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetBusyCouriers
{
    public class GetBusyCouriersHandler : IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>
    {
        private string _connectionString;

        public GetBusyCouriersHandler(string connectionString) 
        {
            _connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                throw new ArgumentException(nameof(connectionString)) :
                connectionString;
        }

        public async Task<GetBusyCouriersResponse> Handle(GetBusyCouriersQuery request,
                                                          CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var querySql = $@"SELECT id, name, location_x, location_y, 
                             status_id, transport_id 
                             FROM public.couriers
                             WHERE status_id=@status_id;";
            var commandDefinition = new CommandDefinition(querySql,
                                                          new { status_id = CourierStatus.Busy },
                                                          cancellationToken: cancellationToken);
            var queryResult = await connection.QueryAsync(command: commandDefinition);

            if (queryResult.AsList().Count == 0)
            {
                return null;
            }

            var couriers = new List<CourierMap>();
            foreach (var result in queryResult)
            {
                couriers.Add(MapToCourier(result));
            }
            return new GetBusyCouriersResponse(couriers);
        }

        private static CourierMap MapToCourier(dynamic result)
        {
            var courier = new CourierMap()
            {
                Id = result[0],
                Name = result[1],
                Location = new LocationMap() 
                { 
                    X = result[2], 
                    Y = result[3] 
                },
                TransportId = result[4]
            };
            return courier;
        }
    }
}
