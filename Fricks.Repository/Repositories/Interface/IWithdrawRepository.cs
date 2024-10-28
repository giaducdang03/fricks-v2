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
    public interface IWithdrawRepository : IGenericRepository<Withdraw>
    {
        public Task<Pagination<Withdraw>> GetWithdrawsPaging(PaginationParameter paginationParameter, WithdrawFilter filter);

        public Task<Withdraw> GetWithdrawsById(int id);
    }
}
