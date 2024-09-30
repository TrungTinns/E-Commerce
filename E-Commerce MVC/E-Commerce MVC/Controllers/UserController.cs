using AutoMapper;
using E_Commerce_MVC.Data;
using E_Commerce_MVC.Helpers;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

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

        #region Sign up
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
        #endregion

        #region Sign in
        [HttpGet]
        public IActionResult SignIn(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid) 
            {
                var users = db.KhachHangs.SingleOrDefault(u => u.MaKh == model.Username);
                if (users == null)
                {
                    ModelState.AddModelError("Error", "User does not exist");
                }
                else
                {
                    if (!users.HieuLuc)
                    {
                        ModelState.AddModelError("Error", "Account has been locked. Please contact admin");
                    }
                    else
                    {
                        if (users.MatKhau != model.Password.ToMd5Hash(users.RandomKey))
                        {
                            ModelState.AddModelError("Error", "Username or Password is incorrect");
                        }
                        else 
                        {
                            var claims = new List<Claim> 
                            {
                                new Claim(ClaimTypes.Email, users.Email),
                                new Claim(ClaimTypes.Name, users.HoTen),
                                new Claim("UserID", users.MaKh),
                                new Claim(ClaimTypes.Role, "Customer"),
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);
                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
            return View();
        }
        #endregion
        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
