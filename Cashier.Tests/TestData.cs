using Cashier.Models;
using Cashier.Models.Database;
using System.Collections.Generic;

namespace Cashier.Tests
{
    internal static class TestData
    {
        internal static List<Order> ExampleOrders()
        {
            var order1 = new Order() { OrderSize = 6 };
            order1.Jobs.AddRange(new List<Job>()
            {
                new Job()
                {
                    Product = ECoffeeType.COFFEE_WITH_MILK,
                    OrderSize = 1
                },
                new Job
                {
                    Product = ECoffeeType.ESPRESSO_CHOCOLATE,
                    OrderSize = 3,
                    Machine = "http://localhost:1337/"
                },
                new Job
                {
                    Product = ECoffeeType.MOCCACHINO,
                    OrderSize = 2
                },
                new Job
                {
                    Product = ECoffeeType.HOT_WATER,
                    OrderSize = 1,
                    Machine = "http://localhost:1337/"
                }
            });

            var order2 = new Order() { OrderSize = 1 };
            order2.Jobs.AddRange(new List<Job>()
            {
                new Job()
                {
                    Product = ECoffeeType.KAKAO,
                    OrderSize = 1,
                    Machine = "http://localhost:1337/"
                }
            });

            return new List<Order>() { order1, order2 };
        }

        internal static CoffeeRequest ExampleRequest()
        {
            var request = new CoffeeRequest();
            request.Coffees.AddRange(new List<Coffee>()
            {
                new Coffee()
                {
                    Count = 3,
                    Product = ECoffeeType.KAKAO
                },
                new Coffee()
                {
                    Count = 1,
                    Product = ECoffeeType.ESPRESSO
                }
            });
            return request;
        }
    }
}
