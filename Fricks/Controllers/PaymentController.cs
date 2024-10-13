using Fricks.Service.BusinessModel.PaymentModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using System;

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
                var uri = HttpContext.Request.Host.ToString();
                var result = await _paymentService.ConfirmVnpayPayment(vnpayResponse);
                if (result)
                {
                    if (uri.Contains("localhost"))
                    {
                        return Redirect("http://localhost:3000/payment?status=paid&order=" + vnpayResponse.vnp_TxnRef);
                    }
                    else
                    {
                        return Redirect("https://frickshop.site/payment?status=paid&order=" + vnpayResponse.vnp_TxnRef);
                    }
                }
                if (uri.Contains("localhost"))
                {
                    return Redirect("http://localhost:3000/payment?status=failed&order=" + vnpayResponse.vnp_TxnRef);
                }
                else
                {
                    return Redirect("https://frickshop.site/payment?status=failed&order=" + vnpayResponse.vnp_TxnRef);
                }
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

        [HttpGet("payos")]
        public async Task<IActionResult> ConfirmPaymentPayOS([FromQuery] PayOSResponseModel payOSResponseModel)
        {
            try
            {
                var uri = HttpContext.Request.Host.ToString();
                var result = await _paymentService.ConfirmPayOSPayment(payOSResponseModel);
                //if (result)
                //{
                //    var response =new ResponseModel
                //    {
                //        HttpCode = StatusCodes.Status200OK,
                //        Message = "Thanh toán đơn hàng thành công"
                //    };
                //    return Ok(response);
                //    //var urlPara = response.ToUrlParameters();
                //    //return Redirect("https://feventopia.vercel.app/payment?" + urlPara);
                //}
                //return BadRequest(new ResponseModel
                //{
                //    HttpCode = StatusCodes.Status400BadRequest,
                //    Message = "Thanh toán đơn hàng thất bại"
                //});
                if (result)
                {
                    if (uri.Contains("localhost"))
                    {
                        return Redirect("http://localhost:3000/payment?status=paid&order=" + payOSResponseModel.id);
                    }
                    else
                    {
                        return Redirect("https://frickshop.site/payment?status=paid&order=" + payOSResponseModel.id);
                    }
                }
                if (uri.Contains("localhost"))
                {
                    return Redirect("http://localhost:3000/payment?status=failed&order=" + payOSResponseModel.id);
                }
                else
                {
                    return Redirect("https://frickshop.site/payment?status=failed&order=" + payOSResponseModel.id);
                }
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
