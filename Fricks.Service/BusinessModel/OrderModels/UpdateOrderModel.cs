using Fricks.Repository.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.OrderModels
{
    public class UpdateOrderModel
    {
        [Required]
        public int Id { get; set; }

        public string? Image { get; set; } = "";

        [Required]
        public OrderStatus Status { get; set; }
    }
}
