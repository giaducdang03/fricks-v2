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
        public int ManagerId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Address { get; set; } = "";

        [Required]
        public string TaxCode { get; set; } = "";

        [MaxLength(20)]
        public string? BankCode { get; set; }

        [MaxLength(20)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Tài khoản ngân hàng chỉ có thể chứa số")]
        public string? AccountNumber { get; set; }

        [MaxLength(100)]
        public string? AccountName { get; set; }

        public string? Image { get; set; }
    }
}
