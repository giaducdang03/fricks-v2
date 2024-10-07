using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.WalletModels
{
    public class TransactionModel : BaseEntity
    {
        public int WalletId { get; set; }

        public string TransactionType { get; set; } = "";

        public int Amount { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public DateTime? TransactionDate { get; set; }

        public string Status { get; set; } = "";
    }
}
