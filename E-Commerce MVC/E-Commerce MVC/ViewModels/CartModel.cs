namespace E_Commerce_MVC.ViewModels
{
    public class CartModel
    {
        public int Quantity { get; set; }
        public double Subtotal { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
