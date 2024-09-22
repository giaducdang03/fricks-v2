using Fricks.Service.BusinessModel.AuthenModels;
using Fricks.Service.Services;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.RequestModels;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fricks.Controllers
{
    [Route("api/authen")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;

        public AuthenController(IUserService userService, IClaimsService claimsService) 
        {
            _userService = userService;
            _claimsService = claimsService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(SignUpModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userService.RegisterAsync(model);
                    var resp = new ResponseModel
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Đã tạo tài khoản thành công. Hãy thử đăng nhập."
                    };
                    return Ok(resp);
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginWithEmail(LoginRequestModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userService.LoginWithEmailPassword(model.Email, model.Password);
                    if (result.HttpCode == StatusCodes.Status200OK)
                    {
                        return Ok(result);
                    }
                    return Unauthorized(result);
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefresToken([FromBody] string jwtToken)
        {
            try
            {
                var result = await _userService.RefreshToken(jwtToken);
                if (result.HttpCode == StatusCodes.Status200OK)
                {
                    return Ok(result);
                }
                return Unauthorized(result);
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

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> RequestResetPassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                var email = _claimsService.GetCurrentUserEmail;
                var result = await _userService.ChangePasswordAsync(email, changePasswordModel);
                if (result)
                {
                    return Ok(new ResponseModel
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Đổi mật khẩu thành công."
                    });
                }
                return BadRequest(new ResponseModel
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Không thể đổi mật khẩu."
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
