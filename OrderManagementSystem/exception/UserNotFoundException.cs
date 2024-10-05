using System;

namespace OrderManagementSystem
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
    }
}
