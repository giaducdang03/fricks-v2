using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.Repositories;
using Fricks.Repository.Repositories.Interface;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.AuthenModels;
using Fricks.Service.BusinessModel.UserModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Utils;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IOtpService otpService, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<bool> ChangePasswordAsync(string email, ChangePasswordModel changePasswordModel)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user != null)
            {
                bool checkPassword = PasswordUtils.VerifyPassword(changePasswordModel.OldPassword, user.PasswordHash);
                if (checkPassword)
                {
                    user.PasswordHash = PasswordUtils.HashPassword(changePasswordModel.NewPassword);
                    _unitOfWork.UsersRepository.UpdateAsync(user);
                    _unitOfWork.Save();
                    return true;
                }
                else
                {
                    throw new Exception("Mật khẩu cũ không đúng.");
                }
            }
            else
            {
                throw new Exception("Tài khoản không tồn tại.");
            }
        }

        public async Task<AuthenModel> ConfirmEmail(ConfirmOtpModel confirmOtpModel)
        {
            try
            {
                bool checkOtp = await _otpService.ValidateOtpAsync(confirmOtpModel.Email, confirmOtpModel.OtpCode);

                if (checkOtp)
                {
                    // return accesstoken

                    var existUser = await _unitOfWork.UsersRepository.GetUserByEmail(confirmOtpModel.Email);

                    if (existUser == null)
                    {
                        return new AuthenModel
                        {
                            HttpCode = 401,
                            Message = "Tài khoản không tồn tại."
                        };
                    }

                    // update confirm email for user
                    existUser.ConfirmEmail = true;
                    _unitOfWork.UsersRepository.UpdateAsync(existUser);

                    var accessToken = GenerateAccessToken(confirmOtpModel.Email, existUser);
                    var refreshToken = GenerateRefreshToken(confirmOtpModel.Email);

                    _unitOfWork.Save();

                    return new AuthenModel
                    {
                        HttpCode = 200,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }

                return new AuthenModel
                {
                    HttpCode = 401,
                    Message = "OTP không hợp lệ."
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ConfirmResetPassword(ConfirmOtpModel confirmOtpModel)
        {
            return await _otpService.ValidateOtpAsync(confirmOtpModel.Email, confirmOtpModel.OtpCode);
        }

        public async Task<bool> ExecuteResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByEmail(resetPasswordModel.Email);
            if (user != null)
            {
                user.PasswordHash = PasswordUtils.HashPassword(resetPasswordModel.Password);
                _unitOfWork.UsersRepository.UpdateAsync(user);
                _unitOfWork.Save();
                return true;
            }
            else
            {
                throw new Exception("Tài khoản không tồn tại.");
            }
        }

        public async Task<UserModel> GetLoginUserInformationAsync(string email)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user != null)
            {
                UserModel userModel = _mapper.Map<UserModel>(user);
                return userModel;
            }
            return null;
        }

        public async Task<AuthenModel> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                var existUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
                if (existUser == null)
                {
                    return new AuthenModel
                    {
                        HttpCode = 401,
                        Message = "Tài khoản không tồn tại."
                    };
                }
                var verifyUser = PasswordUtils.VerifyPassword(password, existUser.PasswordHash);
                if (verifyUser)
                {
                    // check status user
                    if (existUser.Status == UserStatus.BANNED.ToString() || existUser.IsDeleted == true)
                    {
                        return new AuthenModel
                        {
                            HttpCode = 401,
                            Message = "Tài khoản đã bị cấm."
                        };
                    }

                    if (existUser.ConfirmEmail == false)
                    {
                        // send otp email
                        await _otpService.CreateOtpAsync(existUser.Email, "confirm");

                        _unitOfWork.Save();

                        return new AuthenModel
                        {
                            HttpCode = 401,
                            Message = "Bạn phải xác nhận email trước khi đăng nhập vào hệ thống. OTP đã gửi qua email."
                        };
                    }

                    var accessToken = GenerateAccessToken(email, existUser);
                    var refreshToken = GenerateRefreshToken(email);

                    _unitOfWork.Save();

                    return new AuthenModel
                    {
                        HttpCode = 200,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
                return new AuthenModel
                {
                    HttpCode = 401,
                    Message = "Sai mật khẩu."
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<AuthenModel> LoginWithGoogle(string credental)
        {
            string cliendId = _configuration["GoogleCredential:ClientId"];

            if (string.IsNullOrEmpty(cliendId))
            {
                throw new Exception("ClientId is null");
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { cliendId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credental, settings);
            if (payload == null)
            {
                throw new Exception("Credental không hợp lệ.");
            }

            var existUser = await _unitOfWork.UsersRepository.GetUserByEmail(payload.Email);

            if (existUser != null)
            {

                if (existUser.Role != RoleEnums.CUSTOMER.ToString())
                {
                    throw new Exception("Tài khoản của bạn không được phép đăng nhập với Google.");
                }

                if (existUser.Status == UserStatus.BANNED.ToString())
                {
                    throw new Exception("Tài khoản đã bị cấm.");
                }
                else
                {
                    // create accesstoken
                    var accessToken = GenerateAccessToken(existUser.Email, existUser);
                    var refreshToken = GenerateRefreshToken(existUser.Email);

                    _unitOfWork.Save();

                    return new AuthenModel()
                    {
                        HttpCode = 200,
                        Message = "Đăng nhập với Google thành công.",
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
            }
            else
            {
                // create new account
                try
                {
                    User newUser = new User()
                    {
                        Email = payload.Email,
                        FullName = payload.Name,
                        ConfirmEmail = true,
                        UnsignFullName = StringUtils.ConvertToUnSign(payload.Name),
                        Avatar = payload.Picture,
                        Status = UserStatus.ACTIVE.ToString(),
                        GoogleId = payload.JwtId,
                        Role = RoleEnums.CUSTOMER.ToString()
                    };

                    await _unitOfWork.UsersRepository.AddAsync(newUser);

                    // create accesstoken
                    var accessToken = GenerateAccessToken(newUser.Email, newUser);
                    var refreshToken = GenerateRefreshToken(newUser.Email);

                    _unitOfWork.Save();

                    return new AuthenModel()
                    {
                        HttpCode = 200,
                        Message = "Đăng nhập với Google thành công.",
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
                catch
                {
                    throw;
                }

            }
        }

        public async Task<AuthenModel> RefreshToken(string jwtToken)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = authSigningKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                SecurityToken validatedToken;
                var principal = handler.ValidateToken(jwtToken, validationParameters, out validatedToken);
                var email = principal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                if (email != null)
                {
                    var existUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
                    if (existUser != null)
                    {
                        var accessToken = GenerateAccessToken(email, existUser);
                        var refreshToken = GenerateRefreshToken(email);
                        return new AuthenModel
                        {
                            HttpCode = 200,
                            Message = "Refresh token successfully.",
                            AccessToken = accessToken,
                            RefreshToken = refreshToken
                        };
                    }
                }
                return new AuthenModel
                {
                    HttpCode = 401,
                    Message = "Tài khoản không tồn tại."
                };
            }
            catch
            {
                throw new Exception("Token không hợp lệ");
            }
        }

        public async Task<bool> RegisterAsync(SignUpModel model)
        {
            try
            {
                User newUser = new User
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    UnsignFullName = StringUtils.ConvertToUnSign(model.FullName),
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role.ToString(),
                    Status = UserStatus.ACTIVE.ToString(),
                    ConfirmEmail = false
                };

                var existUser = await _unitOfWork.UsersRepository.GetUserByEmail(newUser.Email);

                if (existUser != null)
                {
                    throw new Exception("Tài khoản đã tồn tại.");
                }

                // hash password
                newUser.PasswordHash = PasswordUtils.HashPassword(model.Password);

                await _unitOfWork.UsersRepository.AddAsync(newUser);

                // send otp email
                await _otpService.CreateOtpAsync(newUser.Email, "confirm");

                _unitOfWork.Save();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RequestResetPassword(string email)
        {
            var existUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);

            if (existUser != null)
            {
                if (existUser.ConfirmEmail == true)
                {
                    await _otpService.CreateOtpAsync(email, "reset");
                    _unitOfWork.Save();
                    return true;
                }
            }
            else
            {
                throw new Exception("Tài khoản không tồn tại.");
            }
            return false;
        }

        private string GenerateAccessToken(string email, User user)
        {
            var role = user.Role.ToString();

            var authClaims = new List<Claim>();

            if (role != null)
            {
                authClaims.Add(new Claim(ClaimTypes.Email, email));
                authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var accessToken = GenerateJsonWebToken.CreateToken(authClaims, _configuration, DateTime.UtcNow);
            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        private string GenerateRefreshToken(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
            };
            var refreshToken = GenerateJsonWebToken.CreateRefreshToken(claims, _configuration, DateTime.UtcNow);
            return new JwtSecurityTokenHandler().WriteToken(refreshToken).ToString();
        }
    }
}
