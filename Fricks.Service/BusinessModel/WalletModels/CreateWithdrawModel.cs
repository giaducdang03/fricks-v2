using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.WalletModels
{
    public class CreateWithdrawModel
    {
        [Required]
        [Range(100000, 10000000, ErrorMessage = "Số tiền rút phải từ 100.000 đến 10.000.000 VNĐ")]
        public decimal Amount { get; set; }
    }
}
