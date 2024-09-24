using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly TeeShopContext db;

        public ProductController(TeeShopContext context)
        {
            db = context;
        }
        public IActionResult Index(int? category)
        {
            var products = db.HangHoas.AsQueryable();
            if (category.HasValue) 
            { 
                products = products.Where(p => p.MaLoai == category.Value);
            }

            var result = products.Select(p => new ProductVM
            {
                ProductId = p.MaHh,
                ProductName = p.TenHh,
                Price = p.DonGia ?? 0.0,
                Image = p.Hinh ?? "",
                Description = p.MoTaDonVi ?? "",
                ProductType = p.MaLoaiNavigation.TenLoai,
            });
            return View(result);
        }
        
        public IActionResult Search(string? query)
        {
            var products = db.HangHoas.AsQueryable();
            if (query != null)
            {
                products = products.Where(p => p.TenHh.Contains(query));
            }

            var result = products.Select(p => new ProductVM
            {
                ProductId = p.MaHh,
                ProductName = p.TenHh,
                Price = p.DonGia ?? 0.0,
                Image = p.Hinh ?? "",
                Description = p.MoTaDonVi ?? "",
                ProductType = p.MaLoaiNavigation.TenLoai,
            });
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas.Include(p => p.MaLoaiNavigation).SingleOrDefault(p => p.MaHh == id);  
            if(data == null)
            {
                TempData["Message"] = $"Could not found product";
                return Redirect("/404");
            }
            var result = new ProductDetailVM
            {
                ProductId = data.MaHh,
                ProductName = data.TenHh,
                Price = data.DonGia ?? 0.0,
                Image = data.Hinh ?? string.Empty,
                Description = data.MoTaDonVi ?? string.Empty,
                ProductType = data.MaLoaiNavigation.TenLoai,
                ProductDetail = data.MoTa ?? string.Empty,
                Stock = 10,
                Rating = 5,
            };
            return View(result);
        }
    }
}
