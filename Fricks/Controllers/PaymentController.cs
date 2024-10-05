using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace Fricks.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IPaymentService _paymentService;
        private IClaimsService _claimsService;

        public PaymentController(IPaymentService paymentService, IClaimsService claimsService)
        {
            _paymentService = paymentService;
            _claimsService = claimsService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(List<ItemData> data, int total) 
        {
            try
            {
                var result = await _paymentService.CreatePaymentLink(data, total);
                return Ok(result);
            } catch { throw; }
        }
    }
}
