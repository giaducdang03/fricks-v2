using Fricks.Service.BusinessModel.ProductUnitModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fricks.Controllers
{
    [Route("api/productUnit")]
    [ApiController]
    public class ProductUnitController : ControllerBase
    {
        private IProductUnitService _productUnitService;
        public ProductUnitController(IProductUnitService productUnitService)
        {
            _productUnitService = productUnitService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _productUnitService.GetProductUnitById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("GetAllUnit")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _productUnitService.GetAllProductUnit();
                return Ok(result);
            } catch { throw; }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductUnitProcessModel productUnitModel)
        {
            try
            {
                var result = await _productUnitService.AddProductUnit(productUnitModel);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, ProductUnitProcessModel productUnitModel)
        {
            try
            {
                var result = await _productUnitService.UpdateProductUnit(id, productUnitModel);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productUnitService.DeleteProductUnit(id);
                return Ok(result);
            } catch { throw; }
        }
    }
}
