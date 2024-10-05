using System;
using System.Collections.Generic;
using OrderManagementSystem;
using OrderManagementSystem.dao;

namespace OrderManagementSystem
{
    public class Menu
    {
        private readonly IOrderManagementRepository _orderManagementRepository;
        private User _currentUser;

        public Menu(IOrderManagementRepository orderManagementRepository)
        {
            _orderManagementRepository = orderManagementRepository;
        }

        public void Display()
        {
            while (true)
            {
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void Login()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            // Here you can implement your user authentication logic.
            // For now, we will just assume successful login and assign a role.
            _currentUser = new User(1, username, password, "User"); // Example user

            ShowUserMenu();
        }

        private void Register()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            string role = "User"; // Default role

            User newUser = new User(0, username, password, role);
            _orderManagementRepository.CreateUser(newUser);
            Console.WriteLine("User registered successfully.");
        }

        private void ShowUserMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- User Menu ---");
                Console.WriteLine("1. View Products");
                Console.WriteLine("2. Place Order");
                Console.WriteLine("3. View Orders");
                Console.WriteLine("4. Logout");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewProducts();
                        break;
                    case "2":
                        PlaceOrder();
                        break;
                    case "3":
                        ViewOrders();
                        break;
                    case "4":
                        _currentUser = null; // Logout
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void ViewProducts()
        {
            List<Product> products = _orderManagementRepository.GetAllProducts();
            Console.WriteLine("\n--- Products ---");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}, Type: {product.Type}");
            }
        }

        private void PlaceOrder()
        {
            Console.WriteLine("Enter product IDs to order (comma-separated):");
            string input = Console.ReadLine();
            string[] ids = input.Split(',');

            List<Product> orderProducts = new List<Product>();
            foreach (string id in ids)
            {
                int productId;
                if (int.TryParse(id.Trim(), out productId))
                {
                    // Fetch product by ID and add it to the order list
                    // This should be a method in your repository to get a product by ID.
                    var product = _orderManagementRepository.GetAllProducts().Find(p => p.ProductId == productId);
                    if (product != null)
                    {
                        orderProducts.Add(product);
                    }
                }
            }

            if (orderProducts.Count > 0)
            {
                _orderManagementRepository.CreateOrder(_currentUser, orderProducts);
                Console.WriteLine("Order placed successfully.");
            }
            else
            {
                Console.WriteLine("No valid products selected.");
            }
        }

        private void ViewOrders()
        {
            List<Product> orders = _orderManagementRepository.GetOrderByUser(_currentUser);
            Console.WriteLine("\n--- Your Orders ---");
            foreach (var order in orders)
            {
                Console.WriteLine($"ID: {order.ProductId}, Name: {order.ProductName}, Price: {order.Price}, Type: {order.Type}");
            }
        }
    }
}
