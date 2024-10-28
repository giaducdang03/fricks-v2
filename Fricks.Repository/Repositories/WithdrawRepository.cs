using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
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
    public class WithdrawRepository : GenericRepository<Withdraw>, IWithdrawRepository
    {
        private readonly FricksContext _context;

        public WithdrawRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Withdraw> GetWithdrawsById(int id)
        {
            return await _context.Withdraws.Include(x => x.Wallet).ThenInclude(x => x.Store).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        }

        public async Task<Pagination<Withdraw>> GetWithdrawsPaging(PaginationParameter paginationParameter, WithdrawFilter filter)
        {
            var query = _context.Withdraws.Include(x => x.Wallet).ThenInclude(x => x.Store).Where(x => x.IsDeleted == false).AsQueryable();

            // apply filter
            query = ApplyWithdrawFiltering(query, filter);

            var itemCount = await query.CountAsync();
            var items = await query.Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Withdraw>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        private IQueryable<Withdraw> ApplyWithdrawFiltering(IQueryable<Withdraw> query, WithdrawFilter filter)
        {

            if (filter.WalletId != null && filter.WalletId > 0)
            {
                query = query.Where(s => s.WalletId == filter.WalletId);
            }

            if (filter.WithdrawStatus != null)
            {
                query = query.Where(s => s.Status == filter.WithdrawStatus.ToString());
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "amount":
                        query = filter.Dir?.ToLower() == "desc" ? query.OrderByDescending(s => s.Amount) : query.OrderBy(s => s.Amount);
                        break;
                    case "date":
                        query = filter.Dir?.ToLower() == "desc" ? query.OrderByDescending(s => s.CreateDate) : query.OrderBy(s => s.CreateDate);
                        break;
                    default:
                        query = query.OrderBy(s => s.Id);
                        break;
                }
            }

            return query;
        }
    }
}
