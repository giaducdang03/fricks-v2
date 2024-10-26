using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Enum;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpPost("calculate")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CalculateOrderAsync(CreateOrderModel model)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                model.CustomerEmail = currentEmail;
                var result = await _orderService.CalculateOrderAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("payment")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> PaymentOrderAsync(PaymentOrderModel orderModel)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                orderModel.CustomerEmail = currentEmail;
                var result = await _orderService.RequestPaymentOrderAsync(orderModel, HttpContext);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var result = await _orderService.GetOrderById(id);
                if (result != null)
                {
                    return Ok(result);
                }
                return NotFound(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy đơn hàng"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllOrderPaging([FromQuery] PaginationParameter paginationParameter, [FromQuery] OrderFilter orderFilter)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _orderService.GetOrderPaging(currentEmail, paginationParameter, orderFilter);
                if (result == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status404NotFound,
                        Message = "Không có đơn hàng nào"
                    });
                }

                var metadata = new
                {
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = ex.Message.ToString()
                    }
               );
            }
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN,STORE")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderModel updateOrderModel)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatus(updateOrderModel);
                if (result != null)
                {
                    if (result.Status == OrderStatus.DONE.ToString())
                    {
                        return Ok(new ResponseModel<OrderModel>
                        {
                            Data = result,
                            HttpCode = StatusCodes.Status200OK,
                            Message = "Cập nhật đơn hàng thành công"
                        });
                    }
                    else if (result.Status == OrderStatus.CANCELED.ToString())
                    {
                        return Ok(new ResponseModel<OrderModel>
                        {
                            Data = result,
                            HttpCode = StatusCodes.Status200OK,
                            Message = "Hủy đơn hàng thành công"
                        });
                    }
                }
                return NotFound(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy đơn hàng"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }
    }
}
