using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.BusinessModel.ProductPriceModels;
using Fricks.Service.BusinessModel.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.FavoriteProductModels
{
    public class FavoriteProductModel : BaseEntity
    {
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductImage { get; set; } = "";
        public string BrandName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string StoreName { get; set; } = "";
        public virtual ICollection<ProductPriceModel> ProductPrices { get; set; } = new List<ProductPriceModel>();

        //public ProductModel? Product { get; set; }
        //public UserModel? User { get; set; }
    }
}
