using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Commons.Filters
{
    public class PostFilter : FilterBase
    {
        public string? Title { get; set; }

        public int? ProductId { get; set; }

        public int? StoreId { get; set; }
    }
}
