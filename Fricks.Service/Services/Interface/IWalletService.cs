using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.WalletModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IWalletService
    {
        public Task<WalletModel> GetWalletStoreAsync(string email);

        public Task<Pagination<TransactionModel>> GetTransationsWalletPaginationAsync(PaginationParameter paginationParameter, string email);
    }
}
