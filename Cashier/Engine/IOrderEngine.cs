using System;

namespace Cashier.Engine
{
    public interface IOrderEngine
    {
        void StartOrder(Guid id);
        void StartCoffee(Guid id);
    }
}
