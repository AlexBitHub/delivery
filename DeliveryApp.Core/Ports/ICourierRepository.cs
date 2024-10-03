using DeliveryApp.Core.Domain.CourierAggregate;

namespace DeliveryApp.Core.Ports
{
    public interface ICourierRepository
    {
        Task AddCourierAsync(Courier courier);
        void UpdateCourier(Courier courier);
        Task<Courier> GetCourierAsync(Guid courierId);
        Task<IEnumerable<Courier>> GetFreeCouriersAsync();
    }
}
