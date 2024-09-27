using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using Fricks.Repository.UnitOfWork;
using Fricks.Repository.Utils;
using Fricks.Service.BusinessModel.EmailModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Utils;
using Fricks.Service.Utils.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class OtpService : IOtpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;

        public OtpService(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        public async Task<Otp> CreateOtpAsync(string email, string type, string fullName)
        {
            // default ExpiryTime otp is 5 minutes
            Otp newOtp = new Otp()
            {
                Email = email,
                OtpCode = NumberUtils.GenerateSixDigitNumber().ToString(),
                ExpiryTime = CommonUtils.GetCurrentTime().AddMinutes(5)
            };
            await _unitOfWork.OtpsRepository.AddAsync(newOtp);

            if (type == "confirm")
            {
                bool checkSendMail = await SendOtpAsync(newOtp, fullName);
                if (checkSendMail)
                {
                    return newOtp;
                }
                return null;
            }
            else
            {
                bool checkSendMail = await SendOtpResetPasswordAsync(newOtp, fullName);
                if (checkSendMail)
                {
                    return newOtp;
                }
                return null;
            }
            
        }

        public async Task<bool> ValidateOtpAsync(string email, string otpCode)
        {
            var otpExist = await _unitOfWork.OtpsRepository.GetOtpByCode(otpCode);
            if (otpExist != null)
            {
                if (otpExist.Email == email && otpExist.ExpiryTime > CommonUtils.GetCurrentTime()
                    && otpExist.IsUsed == false)
                {
                    otpExist.IsUsed = true;
                    _unitOfWork.OtpsRepository.UpdateAsync(otpExist);
                    _unitOfWork.Save();
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> SendOtpAsync(Otp otp, string fullName)
        {
            // create new email
            MailRequest newEmail = new MailRequest()
            {
                ToEmail = otp.Email,
                Subject = "Xác thực tài khoản Fricks",
                Body = SendOTPTemplate.EmailSendOTP(otp.Email, otp.OtpCode, fullName)
            };

            // send mail
            await _mailService.SendEmailAsync(newEmail);
            return true;
        }

        private async Task<bool> SendOtpResetPasswordAsync(Otp otp, string fullName)
        {
            // create new email
            MailRequest newEmail = new MailRequest()
            {
                ToEmail = otp.Email,
                Subject = "Đặt lại mật khẩu Fricks",
                Body = SendOTPTemplate.EmailSendOTPResetPassword(otp.Email, otp.OtpCode, fullName)
            };

            // send mail
            await _mailService.SendEmailAsync(newEmail);
            return true;
        }
    }
}
