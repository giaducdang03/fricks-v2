using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public Task<Product?> GetProductByIdAsync(int id);
        public Task<Pagination<Product>> GetProductPagingAsync(PaginationParameter paginationParameter, ProductFilter productFilter);
        public Task<Pagination<Product>> GetProductPaging(Brand? brand, Category? category, PaginationParameter paginationParameter);
        public Task<Pagination<Product>> GetProductByStoreIdPaging(int id, PaginationParameter paginationParameter);
        public Task<Pagination<Product>> GetProductByStoreIdPaging(Brand? brand, Category? category, int id, PaginationParameter paginationParameter);

        public Task<Product> GetProductBySKUAsync(string sku);

        public Task<List<Product>> GetAllProductsAsync();

        public void UpdateProductAsync(Product updateProduct);
    }
}
