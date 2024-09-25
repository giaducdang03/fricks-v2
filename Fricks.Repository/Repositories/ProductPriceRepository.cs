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
    public class ProductPriceRepository : GenericRepository<ProductPrice>, IProductPriceRepository
    {
        private FricksContext _context;
        public ProductPriceRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductPrice>> GetByProductId(int id)
        {
            return await _context.ProductPrices.Where(x => x.ProductId == id && !x.IsDeleted).Include(x => x.Unit).ToListAsync();
        }
    }
}
