using Fricks.Repository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Commons.Filters
{
    public class OrderFilter : FilterBase
    {
        public int? StoreId { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }
    }
}
