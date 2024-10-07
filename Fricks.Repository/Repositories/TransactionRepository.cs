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
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly FricksContext _context;

        public TransactionRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Pagination<Transaction>> GetTransactionsWalletPaging(int walletId, PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Transactions.Where(x => x.WalletId == walletId).CountAsync();
            var items = await _context.Transactions.Where(x => x.WalletId == walletId)
                                    .Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Transaction>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }
    }
}
