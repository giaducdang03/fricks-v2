using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.DashboardModels
{
    public class MainChartAdminModel
    {
        public DateTime Date {  get; set; }

        public int OrderCount { get; set; } = 0;

        public decimal Revenue { get; set; } = 0;
    }
}
