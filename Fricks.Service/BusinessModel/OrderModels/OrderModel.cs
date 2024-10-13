using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.OrderDetailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.OrderModels
{
    public class OrderModel : BaseEntity
    {
        public string? Code { get; set; }

        public int? ShipFee { get; set; }

        public int? Discount { get; set; }

        public int? Total { get; set; }

        public string? Status { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? PaymentMethod { get; set; }

        public string? BankTranNo { get; set; }

        public string? BankCode { get; set; }

        public string? TransactionNo { get; set; }

        public int? UserId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerAddress { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public int? StoreId { get; set; }

        public string? StoreName { get; set; }

        public string? StorePhone { get; set; }

        public string? StoreAddress { get; set; }

        public int? VoucherId { get; set; }
        
        public List<OrderDetailModel>? OrderDetails { get; set; }
    }
}
