using Fricks.Repository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.PaymentModels
{
    public class ConfirmPaymentModel
    {
        public string? BankTranNo { get; set; }

        public string? BankCode { get; set; }

        public string? TransactionNo { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
    }
}
