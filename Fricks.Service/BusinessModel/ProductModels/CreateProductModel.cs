using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.ProductPriceModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.ProductModels
{
    public class CreateProductModel
    {
        [Required]
        [StringLength(10)]
        public string Sku { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        public string? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int StoreId { get; set; }

        [Required]
        public virtual ICollection<CreateProductPriceModel> ProductPrices { get; set; } = new List<CreateProductPriceModel>();
    }

    public class CreateProductPriceModel
    {
        [Required]
        public string UnitCode { get; set; } = "";
        
        [Required]
        public int Price { get; set; }
    }
}
