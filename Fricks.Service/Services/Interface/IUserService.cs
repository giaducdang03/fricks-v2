using Fricks.Service.BusinessModel.AuthenModels;
using Fricks.Service.BusinessModel.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IUserService
    {
        public Task<bool> RegisterAsync(SignUpModel model);

        public Task<AuthenModel> LoginWithEmailPassword(string email, string password);

        public Task<AuthenModel> RefreshToken(string jwtToken);

        public Task<bool> ChangePasswordAsync(string email, ChangePasswordModel changePasswordModel);

        public Task<AuthenModel> ConfirmEmail(ConfirmOtpModel confirmOtpModel);

        public Task<bool> RequestResetPassword(string email);

        public Task<bool> ConfirmResetPassword(ConfirmOtpModel confirmOtpModel);

        public Task<bool> ExecuteResetPassword(ResetPasswordModel resetPasswordModel);

        public Task<UserModel> GetLoginUserInformationAsync(string email);

    }
}
