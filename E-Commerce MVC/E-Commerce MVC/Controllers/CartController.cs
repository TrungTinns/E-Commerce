using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using E_Commerce_MVC.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace E_Commerce_MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly TeeShopContext db;

        public CartController(TeeShopContext context)
        {
            db = context;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddtoCart(int id, int quantity = 1)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if (item == null)
            { 
                var product = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (product == null)
                {
                    TempData["Message"] = $"Could not found product";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    ProductId = product.MaHh,
                    ProductName = product.TenHh,
                    Price = product.DonGia ?? 0.0,
                    Image = product.Hinh ?? string.Empty,
                    Quantity = quantity,
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id) 
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if (item == null) 
            {
                cart.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }
    }
}
