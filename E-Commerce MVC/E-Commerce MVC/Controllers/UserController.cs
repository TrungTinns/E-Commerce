using AutoMapper;
using E_Commerce_MVC.Data;
using E_Commerce_MVC.Helpers;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace E_Commerce_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly TeeShopContext db;
        private readonly IMapper _mapper;

        public UserController(TeeShopContext context, IMapper mapper) 
        {
            db = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(SignUpVM model, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var users = _mapper.Map<KhachHang>(model);
                    users.RandomKey = Util.GenerateRandomKey();
                    users.MatKhau = model.Password.ToMd5Hash(users.RandomKey);
                    users.HieuLuc = true;
                    users.VaiTro = 0;

                    if (Image != null)
                    {
                        users.Hinh = Util.UploadImg(Image, "KhachHang");
                    }
                    db.Add(users);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Product");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                }
            }
            return View();
        }
    }
}
