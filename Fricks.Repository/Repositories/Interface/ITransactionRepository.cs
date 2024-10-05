using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        public Task<Pagination<Transaction>> GetTransactionsWalletPaging(int walletId, PaginationParameter paginationParameter);
    }
}
