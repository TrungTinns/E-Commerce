using E_Commerce_MVC.Data;
using E_Commerce_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using E_Commerce_MVC.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using E_Commerce_MVC.Services;

namespace E_Commerce_MVC.Controllers
{
	public class CartController : Controller
	{
		private readonly TeeShopContext db;
		private readonly PaypalClient _paypalClient;
		private readonly IVnPayService _vnPayService;

		public CartController(TeeShopContext context, PaypalClient paypalClient, IVnPayService vnPayService)
		{
			db = context;
			_paypalClient = paypalClient;
			_vnPayService = vnPayService;
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
			return RedirectToAction("Index", "Product");
		}

		public IActionResult RemoveCart(int id)
		{
			var cart = Cart;
			var item = cart.SingleOrDefault(p => p.ProductId == id);
			if (item != null)
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
			ViewBag.PaypalClientId = _paypalClient.ClientId;
			return View(Cart);
		}

		[Authorize]
		[HttpPost]
		public IActionResult Checkout(CheckoutVM model, string payment = "COD")
		{
			if (ModelState.IsValid)
			{
				if (payment == "VNPay")
				{
					var vnPayModel = new VnPaymentRequestModel()
					{
						Amount = Cart.Sum(p => p.Subtotal),
						CreatedDate = DateTime.Now,
						Description = $"{model.FullName} {model.Phone}",
						FullName = model.FullName,
						OrderId = new Random().Next(1000, 100000),
					};
					return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
				}

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
					foreach (var item in Cart)
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

		[Authorize]
		public IActionResult PaymentSuccess()
		{
			return View("Success");
		}

		#region Paypal payment
		[Authorize]
		[HttpPost("/Cart/create-paypal-order")]
		public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
		{
			var subtotal = Cart.Sum(p => p.Subtotal).ToString();
			var currency = "USD";
			var RefOrderId = "DH" + DateTime.Now.Ticks.ToString();

			try
			{
				var res = await _paypalClient.CreateOrder(subtotal, currency, RefOrderId);
				return Ok(res);
			}
			catch (Exception ex)
			{
				var err = new { ex.GetBaseException().Message };
				return BadRequest(err);
			}
		}

		[Authorize]
		[HttpPost("/Cart/capture-paypal-order")]
		public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
		{
			try
			{
				var res = await _paypalClient.CaptureOrder(orderID);

				var userId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_USERID).Value;
				var user = db.KhachHangs.SingleOrDefault(u => u.MaKh == userId);
				var bill = new HoaDon
				{
					MaKh = userId,
					HoTen = user.HoTen,
					DiaChi = user.DiaChi,
					DienThoai = user.DienThoai,
					NgayDat = DateTime.Now,
					CachThanhToan = "PayPal",
					CachVanChuyen = "",
					MaTrangThai = 0,
					GhiChu = "",
				};

				db.Database.BeginTransaction();
				try
				{
					db.Add(bill);
					db.SaveChanges();

					var detail = new List<ChiTietHd>();
					foreach (var item in Cart)
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
					db.Database.CommitTransaction();

					HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

					return Ok(res);
				}
				catch
				{
					db.Database.RollbackTransaction();
					return BadRequest(new { message = "Error while saving the bill" });
				}


				return BadRequest(new { message = "Payment not completed" });
			}
			catch (Exception ex)
			{
				var err = new { ex.GetBaseException().Message };
				return BadRequest(err);
			}
		}
		#endregion

		[Authorize]
		public IActionResult PaymentFail()
		{
			return View();
		}

		[Authorize]
		public IActionResult PaymentCallback()
		{
			var res = _vnPayService.PaymentExecute(Request.Query);
			if (res == null || res.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Unsuccessfully: {res.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			var userId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_USERID).Value;
			var user = db.KhachHangs.SingleOrDefault(u => u.MaKh == userId);
			var bill = new HoaDon
			{
				MaKh = userId,
				HoTen = user.HoTen,
				DiaChi = user.DiaChi,
				DienThoai = user.DienThoai,
				NgayDat = DateTime.Now,
				CachThanhToan = "VNPay",
				CachVanChuyen = "",
				MaTrangThai = 0,
				GhiChu = res.OrderDescription,
			};

			db.Database.BeginTransaction();
			try
			{
				db.Add(bill);
				db.SaveChanges();

				var detail = new List<ChiTietHd>();
				foreach (var item in Cart)
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
				db.Database.CommitTransaction();

				HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

				TempData["Message"] = $"Successfully";
				return RedirectToAction("PaymentSuccess");
			}
			catch
			{
				db.Database.RollbackTransaction();
				TempData["Message"] = $"Error while saving the bill.";
				return RedirectToAction("PaymentFail");
			}
		}
	}
}
