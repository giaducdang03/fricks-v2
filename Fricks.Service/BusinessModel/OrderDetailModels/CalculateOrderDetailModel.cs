using Fricks.Service.BusinessModel.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.OrderDetailModels
{
    public class CalculateOrderDetailModel
    {
        public int? ProductId { get; set; }

        public int? Price { get; set; }

        public int? Quantity { get; set; }

        public CalculateProductOrderDetailModel? Product { get; set; }
    }
}
