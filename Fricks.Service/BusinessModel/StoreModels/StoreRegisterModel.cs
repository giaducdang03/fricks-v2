using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.StoreModels
{
    public class StoreRegisterModel
    {
        [Required]
        public string ManagerEmail { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Address { get; set; } = "";

        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Số điện thoại không hợp lệ.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Số điện thoại phải có 10 số.")]
        public string PhoneNumber { get; set; } = "";

        [Required]
        public string TaxCode { get; set; } = "";

        [MaxLength(20)]
        [Required]
        public string? BankCode { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Tài khoản ngân hàng chỉ có thể chứa số")]
        public string? AccountNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string? AccountName { get; set; }

        public string? Image { get; set; }
    }
}
