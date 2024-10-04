using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private FricksContext _context;
        public ProductRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(x => x.Brand).Include(x => x.Category)
                                          .Include(x => x.ProductPrices).ThenInclude(x => x.Unit)
                                          .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductBySKUAsync(string sku)
        {
            return await _context.Products.Include(x => x.Brand).Include(x => x.Category)
                                          .Include(x => x.ProductPrices).ThenInclude(x => x.Unit)
                                          .Where(x => x.Sku == sku).FirstOrDefaultAsync();
        }

        public async Task<Pagination<Product>> GetProductByStoreIdPaging(int id, PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Products.CountAsync();
            var items = await _context.Products.Include(x => x.Brand).Include(x => x.Category)
                                    .Include(x => x.ProductPrices).ThenInclude(x => x.Unit)
                                    .Where(x => x.StoreId.Equals(id))
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Product>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<Pagination<Product>> GetProductByStoreIdPaging(Brand? brand, Category? category, int id, PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Products.CountAsync();
            var items = await _context.Products.Include(x => x.Brand).Include(x => x.Category)
                                    .Include(x => x.ProductPrices).ThenInclude(x => x.Unit)
                                    .Where(x => x.StoreId.Equals(id))
                                    .Where(x => brand != null ? brand.Equals(x.Brand) : x.Brand != null 
                                             && category != null ? category.Equals(x.Category) : x.Category != null)
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Product>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<Pagination<Product>> GetProductPaging(PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Products.CountAsync();
            var items = await _context.Products.Include(x => x.Brand).Include(x => x.Category)
                                    .Include(x => x.ProductPrices).ThenInclude(x => x.Unit)
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Product>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<Pagination<Product>> GetProductPaging(Brand? brand, Category? category, PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Products.CountAsync();
            var items = await _context.Products.Include(x => x.Brand).Include(x => x.Category)
                                    .Include(x => x.ProductPrices).ThenInclude(x => x.Unit)
                                    .Where(x => brand != null ? brand.Equals(x.Brand) : x.Brand != null
                                             && category != null ? category.Equals(x.Category) : x.Category != null)
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Product>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }
    }
}
