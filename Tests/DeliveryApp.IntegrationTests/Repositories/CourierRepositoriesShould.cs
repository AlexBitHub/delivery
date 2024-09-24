using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoriesShould : IAsyncLifetime
    {

        private readonly PostgreSqlContainer _postgreContainer = new PostgreSqlBuilder()
            .WithImage("postgre:14.7")
            .WithDatabase("basket")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();

        private ApplicationDbContext _dbContext;

        public CourierRepositoriesShould()
        {

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
                sqlOpt => { sqlOpt.MigrationsAssembly("--------"); }).Options;
            _dbContext = new ApplicationDbContext(contextOptions);
            _dbContext.Database.Migrate();
        }
    }
}
