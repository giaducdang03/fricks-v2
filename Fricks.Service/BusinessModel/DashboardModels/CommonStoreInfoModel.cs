using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.DashboardModels
{
    public class CommonStoreInfoModel
    {
        public decimal Revenue { get; set; } = 0;

        public int NumOfOrders { get; set; } = 0;

        public int NumOfProducts { get; set; } = 0;

        public DateTime LastUpdated { get; set; }
    }
}
