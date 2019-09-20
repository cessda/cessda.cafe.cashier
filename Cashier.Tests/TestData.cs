using Cashier.Models;
using Cashier.Models.Database;
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
                    Coffees = new List<Models.Database.Job>()
                    {
                        new Models.Database.Job()
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

        public static CoffeeRequest ExampleRequest()
        {
            return new CoffeeRequest()
            {
                Coffees = new List<CoffeeRequest.Coffee>()
                {
                    new CoffeeRequest.Coffee()
                    {
                        Count = 3,
                        Product = ECoffeeTypes.KAKAO
                    },
                    new CoffeeRequest.Coffee()
                    {
                        Count = 1,
                        Product = ECoffeeTypes.ESPRESSO
                    }
                }
            };
        }
    }
}
