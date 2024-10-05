using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.BusinessModel.CategoryModels;
using Fricks.Service.BusinessModel.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IProductService
    {
        //public Task<ProductModel> AddProduct(ProductRegisterModel product);
        public Task<ProductModel> AddProduct(CreateProductModel product, string email);
        public Task<ProductModel> UpdateProduct(int id, ProductProcessModel product);
        public Task<ProductModel> DeleteProduct(int id);
        public Task<ProductModel> GetProductById(int id);
        public Task<Pagination<ProductModel>> GetAllProductByStoreIdPagination(int storeId, int brandId, int categoryId, PaginationParameter paginationParameter);
        public Task<Pagination<ProductModel>> GetAllProductPagination(PaginationParameter paginationParameter, ProductFilter productFilter);
    }
}
