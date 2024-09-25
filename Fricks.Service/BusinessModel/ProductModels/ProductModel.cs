using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.BusinessModel.CategoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.ProductModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public int? StoreId { get; set; }
        public int? SoldQuantity { get; set; }
        public BrandModel? Brand { get; set; }
        public CategoryModel? Category { get; set; }
    }
}
