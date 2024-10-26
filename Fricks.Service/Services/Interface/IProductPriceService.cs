using Fricks.Service.BusinessModel.ProductPriceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IProductPriceService
    {
        public Task<ProductPriceModel> AddProductPrice(ProductPriceRegisterModel model);
        public Task<ProductPriceModel> UpdateProductPrice(ProductPriceProcessModel model);
        public Task<ProductPriceModel> DeleteProductPrice(int id);
        public Task<ProductPriceModel> GetProductPriceById(int id);
        public Task<List<ProductPriceModel>> GetProductPriceByProductId(int id);
        public Task<List<ProductPriceModel>> GetAllProductPrice();
    }
}
