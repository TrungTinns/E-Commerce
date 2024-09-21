using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_MVC.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly TeeShopContext db;

        public CategoryMenuViewComponent(TeeShopContext context) => db = context;
        
        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(ctg => new CategoryMenuVM
            {
                ProductId = ctg.MaLoai, 
                ProductName = ctg.TenLoai, 
                Quantity = ctg.HangHoas.Count
            }).OrderBy(p => p.ProductName);
            return View("Default", data);
        }
    }
}
