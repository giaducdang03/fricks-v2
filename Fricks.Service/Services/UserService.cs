using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.Repositories;
using Fricks.Repository.Repositories.Interface;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.AuthenModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration) 
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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

                    //if (existUser.ConfirmEmail == false)
                    //{
                    //    // send otp email
                    //    await _otpService.CreateOtpAsync(existUser.Email, "confirm");

                    //    await transaction.CommitAsync();

                    //    return new AuthenModel
                    //    {
                    //        HttpCode = 401,
                    //        Message = "Bạn phải xác nhận email trước khi đăng nhập vào hệ thống. OTP đã gửi qua email."
                    //    };
                    //}

                    var accessToken = GenerateAccessToken(email, existUser);
                    var refreshToken = GenerateRefreshToken(email);

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
                _unitOfWork.Save();
                return true;
            }
            catch
            {
                throw;
            }
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
