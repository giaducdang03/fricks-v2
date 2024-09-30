using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fricks.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _productService.GetProductById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct([FromQuery] PaginationParameter paginationParameter,
                                                       int brandId, int categoryId)
        {
            try
            {
                var result = await _productService.GetAllProductPagination(brandId, categoryId, paginationParameter);
                var metadata = new
                {
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };
                Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("get-all-store-pagin")]
        public async Task<IActionResult> GetAllProductStore(int storeId, [FromQuery] PaginationParameter paginationParameter,
                                                            int brandId, int categoryId)
        {
            try
            {
                var result = await _productService.GetAllProductByStoreIdPagination(storeId, brandId, categoryId, paginationParameter);
                var metadata = new
                {
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };
                Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
                return Ok(result);
            }
            catch { throw; }
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductRegisterModel model)
        {
            try
            {
                var result = await _productService.AddProduct(model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        public async Task<IActionResult> Update (int id, ProductProcessModel model) 
        {
            try
            {
                var result = await _productService.UpdateProduct(id, model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteProduct(id);
                return Ok(result);
            } catch { throw; }
        }
    }
}
