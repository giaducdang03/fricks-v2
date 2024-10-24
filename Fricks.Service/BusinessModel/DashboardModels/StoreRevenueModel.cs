using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.DashboardModels
{
    public class StoreRevenueModel
    {
        public int StoreId { get; set; } = 0;

        public string StoreName { get; set; } = "";

        public decimal Revenue { get; set; } = 0;

        public DateTime LastUpdated { get; set; }
    }
}
