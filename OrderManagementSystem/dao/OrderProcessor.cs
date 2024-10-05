using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OrderManagementSystem.utility;

namespace OrderManagementSystem.dao
{
    public class OrderProcessor : IOrderManagementRepository
    {
        private readonly string connString;

        public OrderProcessor()
        {
            connString = DbConnUtil.GetConnString();  // Get the connection string from DbConnUtil
        }

        public void CreateUser(User user)
        {
            string query = "INSERT INTO Users (username, password, role) VALUES (@username, @password, @role)";
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@role", user.Role);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateProduct(User user, Product product)
        {
            if (user.Role != "Admin")
            {
                throw new Exception("Only admin users can add products.");
            }

            string query = "INSERT INTO Products (productName, description, price, quantityInStock, type, brand, warrantyPeriod, size, color) " +
                           "VALUES (@productName, @description, @price, @quantityInStock, @type, @brand, @warrantyPeriod, @size, @color)";
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@productName", product.ProductName);
                cmd.Parameters.AddWithValue("@description", product.Description);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@quantityInStock", product.QuantityInStock);
                cmd.Parameters.AddWithValue("@type", product.Type);

                // Handling nullable properties for Electronics and Clothing types
                cmd.Parameters.AddWithValue("@brand", product is Electronics electronics ? electronics.Brand : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@warrantyPeriod", product is Electronics elec ? elec.WarrantyPeriod : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@size", product is Clothing clothing ? clothing.Size : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@color", product is Clothing cloth ? cloth.Color : (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateOrder(User user, List<Product> products)
        {
            int orderId;
            string orderQuery = "INSERT INTO Orders (userId, status) VALUES (@userId, @status); SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(orderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", user.UserId);
                    cmd.Parameters.AddWithValue("@status", "Pending");
                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (var product in products)
                {
                    string orderItemQuery = "INSERT INTO OrderItems (orderId, productId, quantity) VALUES (@orderId, @productId, @quantity)";
                    using (SqlCommand cmd = new SqlCommand(orderItemQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderId", orderId);
                        cmd.Parameters.AddWithValue("@productId", product.ProductId);
                        cmd.Parameters.AddWithValue("@quantity", product.QuantityInStock); // Assuming quantity in stock is the ordered amount
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void CancelOrder(int userId, int orderId)
        {
            string orderQuery = "SELECT * FROM Orders WHERE userId = @userId AND orderId = @orderId";
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(orderQuery, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new OrderNotFoundException("Order not found.");
                }

                reader.Close();

                string cancelOrderQuery = "UPDATE Orders SET status = @status WHERE orderId = @orderId";
                using (SqlCommand cmdCancel = new SqlCommand(cancelOrderQuery, conn))
                {
                    cmdCancel.Parameters.AddWithValue("@status", "Cancelled");
                    cmdCancel.Parameters.AddWithValue("@orderId", orderId);
                    cmdCancel.ExecuteNonQuery();
                }
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            string query = "SELECT * FROM Products";
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product product;
                    if (reader["type"].ToString() == "Electronics")
                    {
                        product = new Electronics(
                            productId: (int)reader["productId"],
                            productName: reader["productName"].ToString(),
                            description: reader["description"].ToString(),
                            price: Convert.ToDouble(reader["price"]),
                            quantityInStock: (int)reader["quantityInStock"],
                            brand: reader["brand"].ToString(),
                            warrantyPeriod: (int)reader["warrantyPeriod"]
                        );
                    }
                    else
                    {
                        product = new Clothing(
                            productId: (int)reader["productId"],
                            productName: reader["productName"].ToString(),
                            description: reader["description"].ToString(),
                            price: Convert.ToDouble(reader["price"]),
                            quantityInStock: (int)reader["quantityInStock"],
                            size: reader["size"].ToString(),
                            color: reader["color"].ToString()
                        );
                    }

                    products.Add(product);
                }

                reader.Close();
            }

            return products;
        }

        public List<Product> GetOrderByUser(User user)
        {
            List<Product> products = new List<Product>();
            string query = "SELECT p.* FROM Products p JOIN OrderItems oi ON p.productId = oi.productId " +
                           "JOIN Orders o ON oi.orderId = o.orderId WHERE o.userId = @userId";
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product product;
                    if (reader["type"].ToString() == "Electronics")
                    {
                        product = new Electronics(
                            productId: (int)reader["productId"],
                            productName: reader["productName"].ToString(),
                            description: reader["description"].ToString(),
                            price: Convert.ToDouble(reader["price"]),
                            quantityInStock: (int)reader["quantityInStock"],
                            brand: reader["brand"].ToString(),
                            warrantyPeriod: (int)reader["warrantyPeriod"]
                        );
                    }
                    else
                    {
                        product = new Clothing(
                            productId: (int)reader["productId"],
                            productName: reader["productName"].ToString(),
                            description: reader["description"].ToString(),
                            price: Convert.ToDouble(reader["price"]),
                            quantityInStock: (int)reader["quantityInStock"],
                            size: reader["size"].ToString(),
                            color: reader["color"].ToString()
                        );
                    }

                    products.Add(product);
                }

                reader.Close();
            }

            return products;
        }
    }
}
