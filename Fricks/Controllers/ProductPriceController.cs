using Fricks.Service.BusinessModel.ProductPriceModels;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fricks.Controllers
{
    [Route("api/product-prices")]
    [ApiController]
    public class ProductPriceController : ControllerBase
    {
        private IProductPriceService _productPriceService;
        public ProductPriceController(IProductPriceService productPriceService)
        {
            _productPriceService = productPriceService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _productPriceService.GetProductPriceById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("get-all-price")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _productPriceService.GetAllProductPrice();
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("get-all-by-product-id")]
        public async Task<IActionResult> GetAllByProductId(int productId)
        {
            try
            {
                var result = await _productPriceService.GetProductPriceByProductId(productId);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductPriceRegisterModel model)
        {
            try
            {
                var result = await _productPriceService.AddProductPrice(model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductPriceProcessModel model) 
        {
            try
            {
                var result = await _productPriceService.UpdateProductPrice(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) 
        {
            try
            {
                var result = await _productPriceService.DeleteProductPrice(id);
                return Ok(result);
            } catch { throw; }
        }
    }
}
