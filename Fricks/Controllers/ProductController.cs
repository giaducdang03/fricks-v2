using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.BusinessModel.UserModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
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
        private IClaimsService _claimsService;

        public ProductController(IProductService productService, IClaimsService claimsService)
        {
            _productService = productService;
            _claimsService = claimsService;
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
        public async Task<IActionResult> GetAllProduct([FromQuery] PaginationParameter paginationParameter, [FromQuery] ProductFilter productFilter)
        {
            try
            {
                var result = await _productService.GetAllProductPagination(paginationParameter, productFilter);
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

        //[HttpGet("get-all-store-pagin")]
        //public async Task<IActionResult> GetAllProductStore(int storeId, [FromQuery] PaginationParameter paginationParameter,
        //                                                    int brandId, int categoryId)
        //{
        //    try
        //    {
        //        var result = await _productService.GetAllProductByStoreIdPagination(storeId, brandId, categoryId, paginationParameter);
        //        var metadata = new
        //        {
        //            result.TotalCount,
        //            result.PageSize,
        //            result.CurrentPage,
        //            result.TotalPages,
        //            result.HasNext,
        //            result.HasPrevious
        //        };
        //        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        //        return Ok(result);
        //    }
        //    catch { throw; }
        //}

        //[HttpPost]
        //public async Task<IActionResult> Add(ProductRegisterModel model)
        //{
        //    try
        //    {
        //        var result = await _productService.AddProduct(model);
        //        return Ok(result);
        //    } catch { throw; }
        //}

        [HttpPost]
        [Authorize(Roles = "ADMIN,STORE")]
        public async Task<IActionResult> CreateUserAsync(CreateProductModel model)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                if (ModelState.IsValid)
                {
                    var result = await _productService.AddProduct(model, currentEmail);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        throw new Exception("Có lỗi trong quá trình tạo sản phẩm mới");
                    }
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel()
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
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
