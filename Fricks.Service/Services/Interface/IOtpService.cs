using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IOtpService
    {
        public Task<Otp> CreateOtpAsync(string email, string type);

        public Task<bool> ValidateOtpAsync(string email, string otpCode);
    }
}
