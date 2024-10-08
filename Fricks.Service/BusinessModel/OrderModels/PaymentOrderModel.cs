using Fricks.Repository.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.OrderModels
{
    public class PaymentOrderModel : CreateOrderModel
    {
        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Số điện thoại không hợp lệ.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Số điện thoại phải có 10 số.")]
        public string CustomerPhone { get; set; } = "";

        [Required]
        public string CustomerAddress { get; set; } = "";

        [Required]
        public PaymentMethod PaymentMethod { get; set; } = 0;
    }
}
