using Fricks.Repository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Commons.Filters
{
    public class WithdrawFilter : FilterBase
    {
        public int? WalletId { get; set; }

        public WithdrawStatus? WithdrawStatus { get; set; }
    }
}
