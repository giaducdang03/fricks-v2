using Fricks.Service.BusinessModel.BannerModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fricks.Controllers
{
    [Route("api/banners")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _bannerService.GetBannerById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _bannerService.GetAllBanner();
                return Ok(result);
            } catch { throw; }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add(BannerProcessModel bannerProcess)
        {
            try
            {
                var result = await _bannerService.AddBanner(bannerProcess);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(BannerUpdateModel bannerUpdateModel)
        {
            try
            {
                var result = await _bannerService.UpdateBanner(bannerUpdateModel);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _bannerService.DeleteBanner(id);
                return Ok(result);
            } catch { throw; }
        }
    }
}
