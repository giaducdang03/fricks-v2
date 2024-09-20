using Fricks.Service.BusinessModel.AuthenModels;
using Fricks.Service.Services.Interface;
using Fricks.ViewModels.RequestModels;
using Fricks.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fricks.Controllers
{
    [Route("api/authen")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenController(IUserService userService) 
        {
            _userService = userService;
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
    }
}
