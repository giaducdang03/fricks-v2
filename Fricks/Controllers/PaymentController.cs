using Fricks.Service.BusinessModel.PaymentModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace Fricks.Controllers
{
    [Route("api/payments")]
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

        [HttpGet("vnpay")]
        public async Task<IActionResult> ConfirmPaymentVnpay([FromQuery] VnPayModel vnpayResponse)
        {
            try
            {
                var result = await _paymentService.ConfirmVnpayPayment(vnpayResponse);
                if (result)
                {
                    return Ok(new ResponseModel
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Thanh toán đơn hàng thành công"
                    });
                }
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Thanh toán đơn hàng thất bại"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }
    }
}
