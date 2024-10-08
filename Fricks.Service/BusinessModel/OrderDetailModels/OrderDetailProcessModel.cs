using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.OrderDetailModels
{
    public class OrderDetailProcessModel
    {
        public int? OrderId { get; set; }

        public int? ProductId { get; set; }

        public string? ProductName { get; set; } //Xin thêm productName để mốt mapping qua PayOS cho dễ ::))))) Đỡ query lại theo id

        public int? Price { get; set; }

        public int? Quantity { get; set; }
    }
}
