using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.BusinessModel.CategoryModels;
using Fricks.Service.BusinessModel.ProductPriceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.ProductModels
{
    public class ProductListModel : BaseEntity
    {
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }
        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public int? SoldQuantity { get; set; }
        public bool? IsAvaiable { get; set; }
        public List<ProductPriceModel>? Price { get; set; }
    }
}
