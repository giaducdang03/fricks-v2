using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.FavoriteProductModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/fav-products")]
    [ApiController]
    public class FavoriteProductController : ControllerBase
    {
        private IFavoriteProductService _favoriteProductService;
        public FavoriteProductController(IFavoriteProductService favoriteProductService)
        {
            _favoriteProductService = favoriteProductService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _favoriteProductService.GetFavoriteProductById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("Get-all-fav-product")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _favoriteProductService.GetAllFavoriteProduct();
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("get-all-fav-product-pagin")]
        public async Task<IActionResult> GetAllPaging([FromQuery] PaginationParameter paginationParameter)
        {
            try
            {
                var result = await _favoriteProductService.GetAllFavoriteProductPagination(paginationParameter);
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

        [HttpPost]
        public async Task<IActionResult> Add(FavoriteProductProcessModel model)
        {
            try
            {
                var result = await _favoriteProductService.AddFavoriteProduct(model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _favoriteProductService.DeleteFavoriteProduct(id);
                return Ok(result);
            } catch { throw; }
        }
    }
}
