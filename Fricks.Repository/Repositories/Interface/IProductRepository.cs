using Fricks.Repository.Commons;
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
        public Task<Pagination<Product>> GetProductPaging(PaginationParameter paginationParameter);
        public Task<Pagination<Product>> GetProductPaging(Brand? brand, Category? category, PaginationParameter paginationParameter);
        public Task<Pagination<Product>> GetProductByStoreIdPaging(int id, PaginationParameter paginationParameter);
        public Task<Pagination<Product>> GetProductByStoreIdPaging(Brand? brand, Category? category, int id, PaginationParameter paginationParameter);
    }
}
