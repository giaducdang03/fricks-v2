using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        private FricksContext _context;
        public VoucherRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Voucher> GetGlobalVoucherByCode(string code)
        {
            return await _context.Vouchers.Include(x => x.Store).FirstOrDefaultAsync(x => x.Code == code && x.Availability == AvailabilityVoucher.GLOBAL.ToString());
        }

        public async Task<Voucher> GetVoucherByCode(string code, int storeId)
        {
            var result = await _context.Vouchers.Include(x => x.Store).FirstOrDefaultAsync(x => x.Code == code && x.StoreId == storeId);
            return result;
        }

        public async Task<Pagination<Voucher>> GetVoucherByStore(int storeId, PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Vouchers.CountAsync();
            var items = await _context.Vouchers.Include(x => x.Store).Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Voucher>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }

        public async Task<Pagination<Voucher>> GetVouchersAsync(PaginationParameter paginationParameter)
        {
            var itemCount = await _context.Vouchers.CountAsync();
            var items = await _context.Vouchers.Include(x => x.Store).Skip((paginationParameter.PageIndex - 1) * paginationParameter.PageSize)
                                    .Take(paginationParameter.PageSize)
                                    .AsNoTracking()
                                    .ToListAsync();
            var result = new Pagination<Voucher>(items, itemCount, paginationParameter.PageIndex, paginationParameter.PageSize);
            return result;
        }
    }
}
