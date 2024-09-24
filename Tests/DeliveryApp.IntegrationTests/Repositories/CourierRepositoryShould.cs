using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryShould : IAsyncLifetime
    {

        private readonly PostgreSqlContainer _postgreContainer = new PostgreSqlBuilder()
            .WithImage("postgre:14.7")
            .WithDatabase("courier")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();

        private ApplicationDbContext _dbContext;
        private Transport _courierTransport;
        private Location _courierLocation;
        private Courier[] _freeCourier = new Courier[2];

        public CourierRepositoryShould()
        {
            _courierTransport = Transport.Pedestrian;
            var location = Location.Create(5, 4);
            location.IsSuccess.Should().BeTrue();
            _courierLocation = location.Value;


        }

        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync().AsTask();
        }

        public async Task InitializeAsync()
        {
            await _postgreContainer.StartAsync();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreContainer.GetConnectionString(),
                sqlOpt => { sqlOpt.MigrationsAssembly("DeliveryApp.Infrastructure"); }).Options;
            _dbContext = new ApplicationDbContext(contextOptions);
            _dbContext.Database.Migrate();
        }

        
        [Fact]
        public async Task AddCourier()
        {
            // Arrange
            var courier = Courier.Create("Володя", _courierTransport, _courierLocation);
            courier.IsSuccess.Should().BeTrue();

            // Act 
            var courierRepository = new CourierRepository(_dbContext);
            await courierRepository.AddCourierAsync(courier.Value);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            // Assert
            var courierFromDb = await courierRepository.GetCourierAsync(courier.Value.Id);
            courierFromDb.Should().BeEquivalentTo(courier.Value);
        }

        [Fact]
        public async Task UpdateCourier()
        {
            // Arrange
            var courier = Courier.Create("Володя", _courierTransport, _courierLocation);
            courier.IsSuccess.Should().BeTrue();

            var courierRepository = new CourierRepository(_dbContext);
            await courierRepository.AddCourierAsync(courier.Value);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            // Act
            var setBusyRes = courier.Value.SetBusy();
            setBusyRes.IsSuccess.Should().BeTrue();
            courierRepository.UpdateCourier(courier.Value);
            await unitOfWork.SaveEntitiesAsync();

            // Assert
            var courierFromDb = await courierRepository.GetCourierAsync(courier.Value.Id);
            courierFromDb.Status.Should().Be(CourierStatus.Busy);

        }

        [Fact]
        public async Task GetCourierById()
        {
            // Arrange
            var courier = Courier.Create("Володя", _courierTransport, _courierLocation);
            courier.IsSuccess.Should().BeTrue();
            
            var courierRepos = new CourierRepository(_dbContext);
            await courierRepos.AddCourierAsync(courier.Value);

            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveEntitiesAsync();

            // Act
            var courierFromDb = await courierRepos.GetCourierAsync(courier.Value.Id);

            // Assert
            courierFromDb.Should().BeEquivalentTo(courier.Value);

        }

        public void GetFreeCouriers()
        {

        }
    }
}
