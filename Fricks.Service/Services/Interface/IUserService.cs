using Fricks.Repository.Commons;
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

        // authen

        public Task<bool> RegisterAsync(SignUpModel model);

        public Task<AuthenModel> LoginWithEmailPassword(string email, string password);

        public Task<AuthenModel> RefreshToken(string jwtToken);

        public Task<bool> ChangePasswordAsync(string email, ChangePasswordModel changePasswordModel);

        public Task<AuthenModel> ConfirmEmail(ConfirmOtpModel confirmOtpModel);

        public Task<bool> RequestResetPassword(string email);

        public Task<bool> ConfirmResetPassword(ConfirmOtpModel confirmOtpModel);

        public Task<bool> ExecuteResetPassword(ResetPasswordModel resetPasswordModel);

        public Task<UserModel> GetLoginUserInformationAsync(string email);

        public Task<AuthenModel> LoginWithGoogle(string credental);
        
        // manager user

        public Task<UserModel> GetUserByIdAsync(int id);

        public Task<Pagination<UserModel>> GetUserPaginationAsync(PaginationParameter paginationParameter);

        public Task<UserModel> CreateUserAsync(CreateUserModel model);

        public Task<UserModel> UpdateUserAsync(UpdateUserModel model);

        public Task<UserModel> DeleteUserAsync(int id, string currentEmail);

    }
}
