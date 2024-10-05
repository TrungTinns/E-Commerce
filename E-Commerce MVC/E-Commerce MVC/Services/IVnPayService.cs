using E_Commerce_MVC.ViewModels;

namespace E_Commerce_MVC.Services
{
    public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
		VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
	}
}