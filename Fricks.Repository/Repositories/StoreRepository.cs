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
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        private FricksContext _context;
        public StoreRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Store> GetStoreByManagerId(int managerId)
        {
            return await _context.Stores.FirstOrDefaultAsync(x => x.ManagerId == managerId && x.IsDeleted == false);
        }

        public async Task<Pagination<Store>> GetStoreByManagerIdPaging(PaginationParameter paginationParameter, int id)
        {
            var itemCount = await _context.Stores.CountAsync();
            var items = await _context.Stores.Include(x => x.Manager).Where(x => x.ManagerId.Equals(id))
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Store>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<Pagination<Store>> GetStorePaging(PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Stores.CountAsync();
            var items = await _context.Stores.Include(x => x.Manager).Where(x => x.IsDeleted == false)
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Store>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }
    }
}
