using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/brand")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id) 
        {
            try
            {
                var result = await _brandService.GetBrandById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("GetAllBrand")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _brandService.GetAllBrand();
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("GetAllBrandPagin")]
        [Authorize]
        public async Task<IActionResult> GetAllPagin([FromQuery] PaginationParameter paginationParameter)
        {
            try
            {
                var result = await _brandService.GetAllBrandPagination(paginationParameter);
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
        [Authorize(Roles = "Admin, Store")]
        public async Task<IActionResult> Add(BrandProcessModel model)
        {
            try
            {
                var result = await _brandService.AddBrand(model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Store")]
        public async Task<IActionResult> Update(int id, BrandProcessModel model)
        {
            try
            {
                var result = await _brandService.UpdateBrand(id, model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Store")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _brandService.DeleteBrand(id);
                return Ok(result);
            } catch { throw; }
        }
    }
}
