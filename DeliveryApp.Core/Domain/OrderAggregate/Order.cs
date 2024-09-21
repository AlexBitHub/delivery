﻿using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    public class Order : Aggregate
    {
        private Order()
        {
        }

        private Order(Guid id, Location location)
        {
            Id = id;
            Location = location;
            Status = OrderStatus.Created;
        }

        public Location Location { get; }
        public OrderStatus Status { get; private set; }
        public Guid? Courierid { get; private set; }

        public static Result<Order, Error> Create(Guid id, Location location)
        {
            if (id == Guid.Empty)
            {
                return GeneralErrors.ValueIsInvalid(nameof(id));
            }
            return new Order(id, location);
        }

        public Result<object, Error> AssignOnCourier(Courier courier)
        {
            if (Status == OrderStatus.Assigned)
            {
                return new Error($"{nameof(Order).ToLowerInvariant()}.has.been.assigned",
                                 "Заказ уже назначен на курьера");
            }
            if (courier.Status == CourierStatus.Busy)
            {
                return new Error($"{nameof(Courier).ToLowerInvariant()}.is.busy",
                                 "Курьер уже имеет заказ.");
            }

            Courierid = courier.Id;
            Status = OrderStatus.Assigned;

            return new object();
        }

        public Result<object, Error> CompleteOrder()
        {
            if (Status != OrderStatus.Assigned)
            {
                return new Error($"{nameof(Order).ToLowerInvariant()}.has.not.yet.been.assigned",
                                 "Заказ еще не был назначен");
            }
            Status = OrderStatus.Completed;

            return new object();
        }
    }
}