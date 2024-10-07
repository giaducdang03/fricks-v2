using Fricks.Repository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.WalletModels
{
    public class UpdateWithdrawModel
    {
        public int Id { get; set; }

        public WithdrawStatus Status { get; set; }

        public string? Note { get; set; }

        public string? ImageTransfer { get; set; }
    }
}
