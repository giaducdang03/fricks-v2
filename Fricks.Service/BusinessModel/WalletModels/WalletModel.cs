using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.WalletModels
{
    public class WalletModel : BaseEntity
    {
        public int? StoreId { get; set; }

        public string? StoreName { get; set; }

        public decimal? Balance { get; set; }
    }
}
