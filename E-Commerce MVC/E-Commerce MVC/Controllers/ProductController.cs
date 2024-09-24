using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    }
}
