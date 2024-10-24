using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.DashboardModels
{
    public class CommonAdminInfoModel
    {
        public decimal Revenue { get; set; } = 0;

        public int NumOfStores { get; set; } = 0;

        public int NumOfProducts { get; set; } = 0;

        public int NumOfUsers { get; set; } = 0;

        public DateTime LastUpdated {  get; set; }
    }
}
