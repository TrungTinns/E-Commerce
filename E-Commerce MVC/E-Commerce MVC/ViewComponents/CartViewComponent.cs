using E_Commerce_MVC.Helpers;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_MVC.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

            return View("CartPanel", new CartModel
            {
                Items = cart, 
                Quantity = cart.Sum(x => x.Quantity),
                Subtotal = cart.Sum(x => x.Subtotal),
            });
        }
    }
}
