using Fricks.Service.BusinessModel.OrderDetailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.OrderModels
{
    public class OrderProcessModel
    {
        public int? ShipFee { get; set; }

        public int? Discount { get; set; }

        public int Total { get; set; }

        public string? PaymentMethod { get; set; } //Dựa vào cái này để chọn flow thanh toán

        public int? UserId { get; set; } //Truyền khách hàng mua dô

        public string? CustomerName { get; set; }

        public string? CustomerAddress { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public int? StoreId { get; set; } //Truyền store dô

        public int? VoucherId { get; set; }

        public List<OrderDetailProcessModel>? OrderDetails { get; set; }
    }
}
