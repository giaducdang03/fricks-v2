using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
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

        public Task<Pagination<TransactionModel>> GetTransationsWalletPaginationAsync(PaginationParameter paginationParameter, string email, TransactionFilter transactionFilter);

        public Task<WithdrawModel> RequestWithdrawStoreAsync(CreateWithdrawModel createWithdrawModel, string email);

        public Task<WithdrawModel> ConfirmWithdrawStoreAsync(UpdateWithdrawModel updateWithdrawModel, string email);

        public Task<WithdrawModel> ProcessWithdrawStoreAsync(UpdateWithdrawModel updateWithdrawModel);
    }
}
