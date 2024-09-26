using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.UserModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fricks.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;

        public UsersController(IUserService userService, IClaimsService claimsService) 
        {
            _userService = userService;
            _claimsService = claimsService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUserAccount([FromQuery] PaginationParameter paginationParameter)
        {
            try
            {
                var result = await _userService.GetUserPaginationAsync(paginationParameter);
                if (result == null)
                {
                    return NotFound(new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status404NotFound,
                        Message = "Account is empty"
                    });
                }

                var metadata = new
                {
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var data = await _userService.GetUserByIdAsync(id);
                if (data == null)
                {
                    return NotFound(new ResponseModel
                    {
                        HttpCode = 404,
                        Message = "Tài khoản không tồn tại."
                    });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseModel
                {
                    HttpCode = 400,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateUserAsync(CreateUserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userService.CreateUserAsync(model);
                    var resp = new ResponseModel()
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Tạo tài khoản thành công. Vui lòng kiểm tra email để đăng nhập vào Fricks."
                    };
                    return Ok(resp);
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
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserModel userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var updateUser = await _userService.UpdateUserAsync(userModel);

                    if (updateUser != null)
                    {
                        return Ok(new ResponseModel
                        {
                            HttpCode = StatusCodes.Status200OK,
                            Message = $"Cập nhật thông tin tài khoản {updateUser.Email} thành công."
                        });
                    }
                    return NotFound(new ResponseModel
                    {
                        HttpCode = StatusCodes.Status404NotFound,
                        Message = "Có lỗi trong quá trình cập nhật thông tin tài khoản."
                    });

                }
                return ValidationProblem(ModelState);

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteAccountById(int id)
        {
            try
            {
                var currentEmail = _claimsService.GetCurrentUserEmail;
                var result = await _userService.DeleteUserAsync(id, currentEmail);
                if (result != null)
                {
                    return Ok(new ResponseModel
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = $"Xóa người dùng {result.Email} thành công."
                    });
                }
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Có lỗi trong quá trình xóa người dùng."
                });
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
    }


}
