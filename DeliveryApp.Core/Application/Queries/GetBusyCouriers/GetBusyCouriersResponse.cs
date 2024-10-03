namespace DeliveryApp.Core.Application.Queries.GetBusyCouriers
{
    public class GetBusyCouriersResponse
    {
        public GetBusyCouriersResponse(List<CourierMap> couriers) 
        {
            Couriers.AddRange(couriers);
        }

        public List<CourierMap> Couriers { get; set; } = new();
    }


    public class CourierMap
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public LocationMap Location { get; set; }
        public int TransportId { get; set; }
    }

    public class LocationMap
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

}
