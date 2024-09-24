namespace E_Commerce_MVC.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string ProductType { get; set; }
    }

    public class ProductDetailVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string ProductType { get; set; }
        public string ProductDetail { get; set; }
        public int Rating { get; set; }
        public int Stock { get; set; }
    }
}
