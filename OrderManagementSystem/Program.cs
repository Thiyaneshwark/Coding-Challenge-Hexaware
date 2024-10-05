using OrderManagementSystem.dao;
using System;

namespace OrderManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IOrderManagementRepository orderManagementRepository = new OrderProcessor(); // Using the OrderProcessor
            Menu menu = new Menu(orderManagementRepository);
            menu.Display();
        }
    }
}
