using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.WalletModels
{
    public class WithdrawModel : BaseEntity
    {
        public int WalletId { get; set; }

        public decimal Amount { get; set; }

        public string? Requester { get; set; }

        public string? Status { get; set; }

        public string? ConfirmBy { get; set; }

        public string? Note { get; set; }

        public DateTime ConfirmDate { get; set; }

        public DateTime TransferDate { get; set; }

        public string? ImageTransfer { get; set; }
    }
}
