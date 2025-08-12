using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IVoucherRepository : IGenericRepository<Voucher>
    {
        public Task<Pagination<Voucher>> GetVoucherByStore(int storeId, PaginationParameter paginationParameter);
        public Task<Pagination<Voucher>> GetVouchersAsync(PaginationParameter paginationParameter);
        public Task<Voucher> GetVoucherByCode(string code, int storeId);
        public Task<Voucher> GetGlobalVoucherByCode(string code);
    }
}
