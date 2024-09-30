using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.BusinessModel.CategoryModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _categoryService.GetCategoryById(id);
                return Ok(result);
            }
            catch { throw; }
        }

        [HttpGet("get-all-category")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _categoryService.GetAllCategory();
                return Ok(result);
            }
            catch { throw; }
        }

        [HttpGet("get-all-category-pagin")]
        public async Task<IActionResult> GetAllPagin([FromQuery] PaginationParameter paginationParameter)
        {
            try
            {
                var result = await _categoryService.GetAllCategoryPagination(paginationParameter);
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
        [Authorize(Roles = "Admin, Store")]
        public async Task<IActionResult> Add(CategoryProcessModel model)
        {
            try
            {
                var result = await _categoryService.AddCategory(model);
                return Ok(result);
            }
            catch { throw; }
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Store")]
        public async Task<IActionResult> Update(int id, CategoryProcessModel model)
        {
            try
            {
                var result = await _categoryService.UpdateCategory(id, model);
                return Ok(result);
            }
            catch { throw; }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Store")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);
                return Ok(result);
            }
            catch { throw; }
        }
    }
}
