using System;

namespace OrderManagementSystem
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(string message) : base(message) { }
    }
}
