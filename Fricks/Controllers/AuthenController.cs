using Fricks.Service.BusinessModel.AuthenModels;
using Fricks.Service.BusinessModel.UserModels;
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
                    var resp = new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Mã OTP đã được gửi về email của bạn. Vui lòng xác thực để đăng nhập."
                    };
                    return Ok(resp);
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
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
                var resp = new ResponseModel<string>
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
                var resp = new ResponseModel<string>
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
                    return Ok(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Đổi mật khẩu thành công."
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Không thể đổi mật khẩu."
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("confirm/email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmOtpModel confirmOtpModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userService.ConfirmEmail(confirmOtpModel);
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
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> RequestResetPassword([FromBody] string email)
        {
            try
            {
                var result = await _userService.RequestResetPassword(email);
                if (result)
                {
                    return Ok(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Mã OTP đặt lại mật khẩu đã được gửi qua email."
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Đặt lại mật khẩu thất bại."
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("reset-password/confirm")]
        public async Task<IActionResult> RequestResetPassword(ConfirmOtpModel confirmOtpModel)
        {
            try
            {
                var result = await _userService.ConfirmResetPassword(confirmOtpModel);
                if (result)
                {
                    return Ok(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Bạn có thể đặt lại mật khẩu ngay bây giờ."
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "OTP không hợp lệ. Hãy thử lại."
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("reset-password/new-password")]
        public async Task<IActionResult> RequestResetPassword(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var result = await _userService.ExecuteResetPassword(resetPasswordModel);
                if (result)
                {
                    return Ok(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Đặt lại mật khẩu thành công."
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Đặt lại mật khẩu thất bại."
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetLoginUserInfo()
        {
            try
            {
                var email = _claimsService.GetCurrentUserEmail;
                var result = await _userService.GetLoginUserInformationAsync(email);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Không thể lấy thông tin tài khoản."
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("login-with-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
        {
            try
            {
                var result = await _userService.LoginWithGoogle(credential);
                if (result.HttpCode == StatusCodes.Status200OK)
                {
                    return Ok(result);
                }
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("confirm/resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] string email)
        {
            try
            {
                var result = await _userService.ResendOtpConfirmAsync(email);
                if (result != null)
                {
                    return Ok(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = $"Mã OTP đã được gửi về email {result.Email}"
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Không thể gửi mã OTP"
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }

        [HttpPost("confirm/cancel")]
        public async Task<IActionResult> CancelEmailConfirm([FromBody] string email)
        {
            try
            {
                var result = await _userService.CancelEmailConfrimAsync(email);
                if (result)
                {
                    return Ok(new ResponseModel<string>
                    {
                        HttpCode = StatusCodes.Status200OK,
                        Message = "Đã hủy tạo tài khoản"
                    });
                }
                return BadRequest(new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = "Không thể hủy tạo tài khoản"
                });
            }
            catch (Exception ex)
            {
                var resp = new ResponseModel<string>
                {
                    HttpCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }
    }
}
