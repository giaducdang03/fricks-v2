using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.UserModels
{
    public class UserModel : BaseEntity
    {
        public string? Email { get; set; }

        public bool? ConfirmEmail { get; set; }

        public string? GoogleId { get; set; }

        public string? Avatar { get; set; }

        public string? FullName { get; set; }

        public string? UnsignFullName { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Role { get; set; }

        public string? Status { get; set; }
    }
}
