using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Entities
{
    public partial class Wallet : BaseEntity
    {
        public int? StoreId { get; set; }

        public decimal? Balance { get; set; }

        public virtual Store? Store { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
