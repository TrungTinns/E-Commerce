﻿using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_MVC.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index(int? category)
        {
            return View();
        }
    }
}
