﻿using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CourierRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddCourierAsync(Courier courier)
        {
            if (courier.Transport != null)
                _dbContext.Attach(courier.Transport);

            if (courier.Status != null)
                _dbContext.Attach(courier.Status);

            await _dbContext.Couriers.AddAsync(courier);
        }

        public async Task<Courier> GetCourierAsync(Guid courierId)
        {
            var courier = await _dbContext.Couriers
                                          .Include(x => x.Transport)
                                          .Include(x => x.Status)
                                          .FirstOrDefaultAsync(c => c.Id == courierId);

            return courier;
        }

        public IEnumerable<Courier> GetFreeCouriers()
        {
            var freeCouriers = _dbContext.Couriers
                                         .Include(x => x.Transport)
                                         .Include(x => x.Status)
                                         .Where(x => x.Status == CourierStatus.Free);

            return freeCouriers;
        }

        public void UpdateCourier(Courier courier)
        {
            if (courier.Transport != null)
                _dbContext.Attach(courier.Transport);
            if (courier.Status != null)
                _dbContext.Attach(courier.Status);

            _dbContext.Couriers.Update(courier);
        }
    }
}