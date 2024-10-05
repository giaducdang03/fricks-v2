using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.FavoriteProductModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/favorites")]
    [ApiController]
    public class FavoriteProductController : ControllerBase
    {
        private IFavoriteProductService _favoriteProductService;
        private IClaimsService _claimsService;

        public FavoriteProductController(IFavoriteProductService favoriteProductService, IClaimsService claimsService)
        {
            _favoriteProductService = favoriteProductService;
            _claimsService = claimsService;
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteProductUser([FromQuery] PaginationParameter paginationParameter)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _favoriteProductService.GetUserFavoriteProductsPagination(currentEmail, paginationParameter);
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
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Add(FavoriteProductProcessModel model)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _favoriteProductService.AddFavoriteProduct(currentEmail, model);
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

        [HttpDelete]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _favoriteProductService.DeleteFavoriteProduct(id);
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
    }
}
