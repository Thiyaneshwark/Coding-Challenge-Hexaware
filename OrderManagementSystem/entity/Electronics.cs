namespace OrderManagementSystem
{
    public class Electronics : Product
    {
        public string Brand { get; set; }
        public int WarrantyPeriod { get; set; }

        public Electronics(int productId, string productName, string description, double price, int quantityInStock, string brand, int warrantyPeriod)
            : base(productId, productName, description, price, quantityInStock, "Electronics")
        {
            Brand = brand;
            WarrantyPeriod = warrantyPeriod;
        }
    }
}
