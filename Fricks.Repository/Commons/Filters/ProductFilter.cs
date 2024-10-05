using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Commons.Filters
{
    public class ProductFilter : FilterBase
    {
        public int? StoreId { get; set; }

        public int? BrandId { get; set; }

        public int? CategoryId { get; set; }
    }
}
