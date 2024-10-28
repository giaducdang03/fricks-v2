using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.DashboardModels
{
    public class CategoryRevenueModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = "";

        public decimal Revenue { get; set; } = 0;

        public DateTime LastUpdated { get; set; }
    }
}
