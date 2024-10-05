using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.WalletModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<TransactionModel>> GetTransationsWalletPaginationAsync(PaginationParameter paginationParameter, string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser != null)
            {
                var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(currentUser.Id);
                if (store != null)
                {
                    var storeWallet = await _unitOfWork.WalletRepository.GetWalletStoreAsync(store.Id);
                    if (storeWallet != null)
                    {
                        var transactions = await _unitOfWork.TransactionRepository.GetTransactionsWalletPaging(storeWallet.Id, paginationParameter);
                        return _mapper.Map<Pagination<TransactionModel>>(transactions);
                    }
                    throw new Exception("Ví không tồn tại");
                }
                throw new Exception("Cửa hàng không tồn tại");
            }
            throw new Exception("Tài khoản không tồn tại");
        }

        public async Task<WalletModel> GetWalletStoreAsync(string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser != null)
            {
                var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(currentUser.Id);
                if (store != null)
                {
                    var storeWallet = await _unitOfWork.WalletRepository.GetWalletStoreAsync(store.Id);
                    if (storeWallet != null)
                    {
                        return _mapper.Map<WalletModel>(storeWallet);
                    }
                    return null;
                }
                throw new Exception("Cửa hàng không tồn tại");
            }
            throw new Exception("Tài khoản không tồn tại");
        }
    }
}
