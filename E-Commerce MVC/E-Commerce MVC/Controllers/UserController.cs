using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace E_Commerce_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly TeeShopContext db;

        public UserController(TeeShopContext context) 
        {
            db = context;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var users = model;
            }
            return View();
        }
    }
}
