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
    public class FavoriteProductRepository : GenericRepository<FavoriteProduct>, IFavoriteProductRepository
    {
        private FricksContext _context;
        public FavoriteProductRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pagination<FavoriteProduct>> GetFavoriteProductPaging(int userid, PaginationParameter paginationParameter)
        {
            var itemCount = await _context.FavoriteProducts.CountAsync();
            var items = await _context.FavoriteProducts.Include(x => x.User)
                                        .Include(x => x.Product)
                                        .Include(x => x.Product.Brand)
                                        .Include(x => x.Product.Category)
                                        .Include(x => x.Product.Store)
                                        .Include(x => x.Product).ThenInclude(x => x.ProductPrices)
                                    .Where(x => x.UserId.Equals(userid))
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<FavoriteProduct>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<List<FavoriteProduct>> GetUserFavoriteProductList(int userid)
        {
            return await _context.FavoriteProducts.Include(x => x.User)
                .Include(x => x.Product).Include(x => x.Product.Brand).Include(x => x.Product.Category).Where(x => x.UserId == userid).ToListAsync();
        }
    }
}
