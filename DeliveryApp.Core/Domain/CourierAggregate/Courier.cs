using CSharpFunctionalExtensions;
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
        public Location Location { get; private set;}
        public CourierStatus Status { get; private set; }

        public static Result<Courier, Error> Create(string name,
                                                    Transport transport,
                                                    Location location)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GeneralErrors.ValueIsRequired(nameof(name));
            }
            if (transport is null)
            {
                return GeneralErrors.ValueIsRequired(nameof(transport));
            }
            if (location is null)
            {
                return GeneralErrors.ValueIsRequired(nameof(location));
            }

            return new Courier(name, transport, location);
        }

        public UnitResult<Error> SetBusy()
        {
            if (Status == CourierStatus.Busy)
            {
                return new Error($"{nameof(Courier).ToLowerInvariant()}.is.already.busy",
                                 "Курьер уже занят");
            }
            Status = CourierStatus.Busy;
            return UnitResult.Success<Error>();
        }

        public UnitResult<Error> SetFree()
        {
            if (Status == CourierStatus.Free)
            {
                return new Error($"{nameof(Courier).ToLowerInvariant()}.is.already.free",
                                 $"Курьер уже свободен");
            }
            Status = CourierStatus.Free;
            return UnitResult.Success<Error>();
        }

        public Result<double, Error> CalculateTimeToDestionation(Location orderLocation)
        {
            double distance = orderLocation - Location;
            return distance / Transport.Speed;
        }

        public UnitResult<Error> Move(Location targetLocation)
        {
            if (targetLocation == null) 
                return GeneralErrors.ValueIsRequired(nameof(targetLocation));

            var difX = targetLocation.X - Location.X;
            var difY = targetLocation.Y - Location.Y;

            var newX = Location.X;
            var newY = Location.Y;

            var speed = Transport.Speed;

            if (difX > 0)
            {
                if (difX >= speed)
                {
                    newX += speed;
                    Location = Location.Create(newX, newY).Value;
                    return UnitResult.Success<Error>();
                }

                if (difX < speed)
                {
                    newX += difX;
                    Location = Location.Create(newX, newY).Value;
                    if (Location == targetLocation) 
                        return UnitResult.Success<Error>();
                    speed -= difX;
                }
            }

            if (difX < 0)
            {
                if (Math.Abs(difX) >= speed)
                {
                    newX -= speed;
                    Location = Location.Create(newX, newY).Value;
                    return UnitResult.Success<Error>();
                    }

                if (Math.Abs(difX) < speed)
                {
                    newX -= Math.Abs(difX);
                    Location = Location.Create(newX, newY).Value;
                    if (Location == targetLocation) 
                        return UnitResult.Success<Error>();
                    speed -= Math.Abs(difX);
                }
            }

            if (difY > 0)
            {
                if (difY >= speed)
                {
                    newY += speed;
                    Location = Location.Create(newX, newY).Value;
                    return UnitResult.Success<Error>();
                }

                if (difY < speed)
                {
                    newY += difY;
                    Location = Location.Create(newX, newY).Value;
                    if (Location == targetLocation) 
                        return UnitResult.Success<Error>();
                }
            }

            if (difY < 0)
            {
                if (Math.Abs(difY) >= speed)
                {
                    newY -= speed;
                    Location = Location.Create(newX, newY).Value;
                    return UnitResult.Success<Error>();
                }

                if (Math.Abs(difY) < speed)
                {
                    newY -= Math.Abs(difY);
                    Location = Location.Create(newX, newY).Value;
                    if (Location == targetLocation) return UnitResult.Success<Error>();
                }
            }

            Location = Location.Create(newX, newY).Value;
            return UnitResult.Success<Error>();
        }

    }
}
