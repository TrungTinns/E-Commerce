using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using E_Commerce_MVC.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpGet]
        public IActionResult Checkout() 
        {
            if (Cart.Count == 0)
            {
                return RedirectToAction("/");
            }
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_USERID).Value;
                var user = new KhachHang();
                if (model.Related)
                {
                    user = db.KhachHangs.SingleOrDefault(u => u.MaKh == userId);
                }
                var bill = new HoaDon
                {
                    MaKh = userId,
                    HoTen = model.FullName ?? user.HoTen,
                    DiaChi = model.Address ?? user.DiaChi,
                    DienThoai = model.Phone ?? user.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "",
                    MaTrangThai = 0,
                    GhiChu = model.Note,
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(bill);
                    db.SaveChanges();

                    var detail = new List<ChiTietHd>();
                    foreach(var item in Cart)
                    {
                        detail.Add(new ChiTietHd
                        {
                            MaHd = bill.MaHd,
                            SoLuong = item.Quantity,
                            DonGia = item.Price,
                            MaHh = item.ProductId,
                            GiamGia = 0,
                        });
                    }
                    db.AddRange(detail);
                    db.SaveChanges();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
              
            }
            return View(Cart);
        }
    }
}
