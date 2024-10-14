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
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly FricksContext _context;

        public TransactionRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pagination<Transaction>> GetTransactionsWalletPaging(int walletId, PaginationParameter paginationParameter, TransactionFilter filter)
        {
            var query = _context.Transactions.Where(x => x.WalletId == walletId).AsQueryable();

            // apply filter
            query = ApplyTransactionFiltering(query, filter);

            var itemCount = await query.CountAsync();
            var items = await query.Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Transaction>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        private IQueryable<Transaction> ApplyTransactionFiltering(IQueryable<Transaction> query, TransactionFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
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
