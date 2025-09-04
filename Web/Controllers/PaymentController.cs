using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPayPalService _payPalService;

        public PaymentController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string bookingId, decimal amount)
        {
            var paymentDto = await _payPalService.CreatePayment(amount, bookingId);
            return Redirect(paymentDto.ApproveUrl!); 
        }

        [HttpGet]
        public async Task<IActionResult> Success(string token, string PayerID)
        {
            var result = await _payPalService.CapturePaymentAsync(token);

            ViewBag.PaypalResponse = result;
            return View();  // Views/Payment/Success.cshtml
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            return View(); // Views/Payment/Cancel.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(new List<object>());
        }
    }
}
