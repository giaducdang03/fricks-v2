using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.VoucherModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IVoucherService
    {
        public Task<Pagination<VoucherModel>> GetAllVoucherByStoreId(int storeId, PaginationParameter paginationParameter);
        public Task<VoucherModel> GetById(int id);
        public Task<VoucherModel> AddVoucher(VoucherProcessModel voucherProcessModel, string username);
        public Task<VoucherModel> UpdateVoucher(int id, VoucherProcessModel voucherProcessModel, string email);
        public Task<VoucherModel> DisableVoucher(int id, string email);
        public Task<VoucherModel> DeleteVoucher(int id, string email);
    }
}
