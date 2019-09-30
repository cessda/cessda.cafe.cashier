using Cashier.Models;
using Cashier.Models.Database;
using System.Collections.Generic;

namespace Cashier.Tests
{
    internal static class TestData
    {
        internal static List<Order> ExampleOrders()
        {
            return new List<Order>()
            {
                new Order()
                {
                    Coffees = new List<Job>()
                    {
                        new Job()
                        {
                            Product = ECoffeeTypes.COFFEE_WITH_MILK,
                            OrderSize = 1
                        },
                        new Job
                        {
                            Product = ECoffeeTypes.ESPRESSO_CHOCOLATE,
                            OrderSize = 3,
                            State = ECoffeeState.PROCESSED
                        },
                        new Job
                        {
                            Product = ECoffeeTypes.MOCCACHINO,
                            OrderSize = 2,
                            State = ECoffeeState.QUEUED
                        },
                        new Job
                        {
                            Product = ECoffeeTypes.HOT_WATER,
                            OrderSize = 1,
                            State = ECoffeeState.PROCESSED
                        }
                    },
                    OrderSize = 6
                },
                new Order()
                {
                    Coffees = new List<Job>()
                    {
                        new Job()
                        {
                            Product = ECoffeeTypes.KAKAO,
                            OrderSize = 1,
                            State = ECoffeeState.PROCESSED
                        }
                    }
                }
            };
        }

        internal static CoffeeRequest ExampleRequest()
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
