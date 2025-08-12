using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.VoucherModels
{
    public class VoucherModel
    {
        public int Id { get; set; }

        public int? StoreId { get; set; }
        public string? StoreName { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }

        public int? DiscountPercent { get; set; }

        public int? MaxDiscount { get; set; }

        public int? MinOrderValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        public string? Availability { get; set; }

        public string? Status { get; set; }
    }
}
