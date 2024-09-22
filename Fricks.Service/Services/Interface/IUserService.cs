using Fricks.Service.BusinessModel.AuthenModels;
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

    }
}
