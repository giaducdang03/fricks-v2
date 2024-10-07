using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Fricks.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<IActionResult> Add(OrderProcessModel model)
        {
            try
            {
                var result = await _orderService.AddOrder(model);
                return Ok(result);
            } catch { throw; }
        }
    }
}
