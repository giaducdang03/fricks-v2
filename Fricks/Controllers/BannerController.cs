using Fricks.Service.BusinessModel.BannerModels;
using Fricks.Service.Services.Interface;
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
        public async Task<IActionResult> Add(BannerProcessModel bannerProcess)
        {
            try
            {
                var result = await _bannerService.AddBanner(bannerProcess);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, BannerProcessModel bannerProcess)
        {
            try
            {
                var result = await _bannerService.UpdateBanner(id, bannerProcess);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
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
