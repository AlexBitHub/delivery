﻿using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    public class Courier : Aggregate
    {
        private Courier() 
        {
        }

        private Courier(string name, Transport transport, Location location) : this()
        {
            Id = Guid.NewGuid();
            Transport = transport;
            Location = location;
            Status = CourierStatus.Free;
        }

        public string Name { get; }
        public Transport Transport { get; }
        public Location Location { get; }
        public CourierStatus Status { get; private set; }

        public static Result<Courier, Error> Create(string name,
                                                    Transport transport,
                                                    Location location)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GeneralErrors.ValueIsRequired(nameof(name));
            }
            return new Courier(name, transport, location);
        }

        public Result<object, Error> SetBusy()
        {
            if (Status == CourierStatus.Busy)
            {
                return new Error($"{nameof(Courier).ToLowerInvariant()}.is.already.busy",
                                 "Курьер уже занят");
            }
            Status = CourierStatus.Busy;
            return new object();
        }

        public Result<object, Error> SetFree()
        {
            if (Status == CourierStatus.Free)
            {
                return new Error($"{nameof(Courier).ToLowerInvariant()}.is.already.free",
                                 $"Курьер уже свободен");
            }
            Status = CourierStatus.Free;
            return new object();
        }

        public Result<double, Error> CalculateTimeToDestionation(Location orderLocation)
        {
            double distance = orderLocation - Location;
            return distance / Transport.Speed;
        }
    }
}