using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.StoreModels;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
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
            }
            catch { throw; }
        }

        [HttpGet("manager/{userId}")]
        [Authorize(Roles = "ADMIN,STORE")]
        public async Task<IActionResult> GetByManagerId(int userId)
        {
            try
            {
                var result = await _storeService.GetStoreByManagerId(userId);
                if (result != null)
                {
                    return Ok(result);
                }
                return NotFound(new ResponseModel()
                {
                    HttpCode = StatusCodes.Status404NotFound,
                    Message = "Tài khoản này chưa quản lí cửa hàng nào"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = ex.Message.ToString()
                    }
               );
            }
        }

        [HttpGet]
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
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = ex.Message.ToString()
                    }
               );
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add(StoreRegisterModel model)
        {
            try
            {
                var result = await _storeService.AddStore(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = ex.Message.ToString()
                    }
               );
            }
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN,STORE")]
        public async Task<IActionResult> Update(int id, StoreProcessModel model)
        {
            try
            {
                var result = await _storeService.UpdateStore(id, model);
                return Ok(new ResponseModel
                {
                    HttpCode = StatusCodes.Status200OK,
                    Message = $"Cập nhật cửa hàng {result.Name} thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = ex.Message.ToString()
                    }
               );
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _storeService.DeleteStore(id);
                return Ok(new ResponseModel
                {
                    HttpCode = StatusCodes.Status200OK,
                    Message = $"Xóa cừa hàng {result.Name} thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status400BadRequest,
                        Message = ex.Message.ToString()
                    }
               );
            }
        }
    }
}
