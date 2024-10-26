using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.UserModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.RequestModels;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        private readonly IClaimsService _claimsService;

        public StatisticsController(IStatisticService statisticService, IClaimsService claimsService) 
        {
            _statisticService = statisticService;
            _claimsService = claimsService;
        }

        [HttpGet("admin/info")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetStatisticsAdmin()
        {
            try
            {
                var result = await _statisticService.GetCommonInfoAdminAsync();
                if (result == null)
                {
                    throw new Exception("Có lỗi trong quá trình lấy dữ liệu");
                }
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

        [HttpGet("admin/main-chart")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetMainChartInfoAdmin()
        {
            try
            {
                var result = await _statisticService.GetMainChartAdminInfoAsync();
                if (result == null)
                {
                    throw new Exception("Có lỗi trong quá trình lấy dữ liệu");
                }
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

        [HttpGet("admin/revenue-category")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetRevenueCategoryInfoAdmin([FromQuery] DateRequestModel dateRequestModel)
        {
            try
            {
                var result = await _statisticService.GetCategoryRevenueAdminAsync(dateRequestModel.Month, dateRequestModel.Year);
                if (result == null)
                {
                    throw new Exception("Có lỗi trong quá trình lấy dữ liệu");
                }
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

        [HttpGet("admin/revenue-store")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetRevenueStoreInfoAdmin([FromQuery] DateRequestModel dateRequestModel)
        {
            try
            {
                var result = await _statisticService.GetStoreRevenueModelAdminAsync(dateRequestModel.Month, dateRequestModel.Year);
                if (result == null)
                {
                    throw new Exception("Có lỗi trong quá trình lấy dữ liệu");
                }
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

        [HttpGet("admin/featured-products")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetFeaturedProductsInfoAdmin()
        {
            try
            {
                var result = await _statisticService.GetFeaturedProductsAsync();
                if (result == null)
                {
                    throw new Exception("Có lỗi trong quá trình lấy dữ liệu");
                }
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

        [HttpGet("store/info")]
        [Authorize(Roles = "STORE")]
        public async Task<IActionResult> GetStatisticsStore()
        {
            try
            {
                var email = _claimsService.GetCurrentUserEmail;
                var result = await _statisticService.GetCommonInfoStoreAsync(email);
                if (result == null)
                {
                    throw new Exception("Có lỗi trong quá trình lấy dữ liệu");
                }
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
    }
}
