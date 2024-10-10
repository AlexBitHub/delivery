namespace DeliveryApp.Core.Application.Queries.GetOpenOrders
{
    public class GetCreatedAndAssignedOrdersResponse
    {
        public GetCreatedAndAssignedOrdersResponse(List<OrderMap> orderMaps)
        {
            OrderMaps.AddRange(orderMaps);
        }

        public List<OrderMap> OrderMaps { get; set; }
    }

    public class OrderMap
    {
        public Guid Id { get; set; }
        public LocationMap Location { get; set; }
    }

    public class LocationMap
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
