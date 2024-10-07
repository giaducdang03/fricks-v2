using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Fricks.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderService _orderService;
        private IClaimsService _claimsService;

        public OrderController(IOrderService orderService, IClaimsService claimsService)
        {
            _orderService = orderService;
            _claimsService = claimsService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateOrderAsync(CreateOrderModel model)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                model.CustomerEmail = currentEmail;
                var result = await _orderService.CreateOrderAsync(model);
                return Ok(result);
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

        [HttpPost("confirm")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ConfirmOrderAsync(ConfirmOrderModel orderModel)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _orderService.ConfirmOrderAsync(orderModel, currentEmail);
                return Ok(result);
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

        [HttpPost("cancel")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CancelOrderAsync(ConfirmOrderModel confirmOrder)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _orderService.CancelOrderAsync(confirmOrder, currentEmail);
                if (result)
                {
                    return Ok(new ResponseModel
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Bạn đã hủy đơn hàng thành công"
                    });
                }
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Có lỗi trong quá trình hủy đơn hàng"
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
