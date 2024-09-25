﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.UserModels
{
    public class UpdateUserModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Số điện thoại phải có 10 số")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime Dob { get; set; }

        public int? Gender { get; set; }

        public string? Avatar { get; set; }
    }
}
