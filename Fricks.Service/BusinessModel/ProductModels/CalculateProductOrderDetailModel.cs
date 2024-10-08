using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.ProductModels
{
    public class CalculateProductOrderDetailModel
    {
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? StoreName { get; set; }
        public int? UnitId { get; set; }
        public string? UnitName { get; set; }
    }
}
