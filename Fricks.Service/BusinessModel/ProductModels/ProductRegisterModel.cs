using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.ProductModels
{
    public class ProductRegisterModel
    {
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public int StoreId { get; set; }
        public int? SoldQuantity { get; set; }
    }
}
