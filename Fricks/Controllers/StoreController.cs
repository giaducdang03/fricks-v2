using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.StoreModels;
using Fricks.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _storeService.GetStoreById(id);
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("get-all-store-by-manager-id")]
        public async Task<IActionResult> GetByManagerId(int userId, [FromQuery] PaginationParameter paginationParameter) 
        {
            try
            {
                var result = await _storeService.GetStoreByManagerId(paginationParameter, userId);
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

        [HttpGet("get-all-store")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _storeService.GetAllStore();
                return Ok(result);
            } catch { throw; }
        }

        [HttpGet("get-all-store-pagin")]
        public async Task<IActionResult> GetAllPagin([FromQuery] PaginationParameter paginationParameter)
        {
            try
            {
                var result = await _storeService.GetAllStorePagination(paginationParameter);
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
        public async Task<IActionResult> Add(StoreRegisterModel model)
        {
            try
            {
                var result = await _storeService.AddStore(model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, StoreProcessModel model)
        {
            try
            {
                var result = await _storeService.UpdateStore(id, model);
                return Ok(result);
            } catch { throw; }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _storeService.DeleteStore(id);
                return Ok(result);
            } catch { throw; }
        }
    }
}
