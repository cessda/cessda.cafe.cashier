using Cashier.Models;
using System;
using System.Collections.Generic;

namespace Cashier.Tests
{
    public class TestData
    {
        public static List<Order> ExampleOrders()
        {
            return new List<Order>()
            {
                new Order()
                {
                    Coffees = new List<Coffee>()
                    {
                        new Coffee()
                        {
                            JobId = Guid.NewGuid(),
                            Product = ECoffeeTypes.COFFEE_WITH_MILK,
                            OrderSize = 1
                        }
                    },
                    OrderId = Guid.NewGuid(),
                    OrderSize = 1
                }
            };
        }
    }
}
