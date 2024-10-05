namespace OrderManagementSystem
{
    public class Clothing : Product
    {
        public string Size { get; set; }
        public string Color { get; set; }

        public Clothing(int productId, string productName, string description, double price, int quantityInStock, string size, string color)
            : base(productId, productName, description, price, quantityInStock, "Clothing")
        {
            Size = size;
            Color = color;
        }
    }
}
