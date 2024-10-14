using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Repository.Utils;
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

        public async Task<WithdrawModel> ConfirmWithdrawStoreAsync(UpdateWithdrawModel updateWithdrawModel, string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser != null && currentUser.Role.ToUpper() == RoleEnums.ADMIN.ToString())
            { 
                if (updateWithdrawModel.Status == WithdrawStatus.APPROVED)
                {
                    var existWithdraw = await _unitOfWork.WithdrawRepository.GetByIdAsync(updateWithdrawModel.Id);
                    if (existWithdraw != null)
                    {
                        var storeWallet = await _unitOfWork.WalletRepository.GetByIdAsync(existWithdraw.WalletId);
                        if (storeWallet != null)
                        {
                            // check wallet
                            if (storeWallet.Balance < existWithdraw.Amount)
                            {
                                existWithdraw.Status = WithdrawStatus.REJECTED.ToString();
                                existWithdraw.Note = "Số dư của ví không đủ để thực hiện giao dịch";
                                existWithdraw.ConfirmBy = currentUser.Email;
                                existWithdraw.ConfirmDate = CommonUtils.GetCurrentTime();

                                _unitOfWork.WithdrawRepository.UpdateAsync(existWithdraw);
                                _unitOfWork.Save();
                                return _mapper.Map<WithdrawModel>(existWithdraw);
                            }

                            existWithdraw.Status = WithdrawStatus.APPROVED.ToString();
                            existWithdraw.Note = updateWithdrawModel.Note;
                            existWithdraw.ConfirmBy = currentUser.Email;
                            existWithdraw.ConfirmDate = CommonUtils.GetCurrentTime();

                            _unitOfWork.WithdrawRepository.UpdateAsync(existWithdraw);
                            _unitOfWork.Save();
                            return _mapper.Map<WithdrawModel>(existWithdraw);
                        }
                        else
                        {
                            throw new Exception("Ví không tồn tại");
                        }
                    }
                    else
                    {
                        throw new Exception("Yêu cầu rút tiền không tồn tại");
                    }
                }
                else
                {
                    var existWithdraw = await _unitOfWork.WithdrawRepository.GetByIdAsync(updateWithdrawModel.Id);
                    if (existWithdraw != null)
                    {
                        existWithdraw.Status = WithdrawStatus.REJECTED.ToString();
                        existWithdraw.Note = updateWithdrawModel.Note;
                        existWithdraw.ConfirmBy = currentUser.Email;
                        existWithdraw.ConfirmDate = CommonUtils.GetCurrentTime();

                        _unitOfWork.WithdrawRepository.UpdateAsync(existWithdraw);
                        _unitOfWork.Save();
                        return _mapper.Map<WithdrawModel>(existWithdraw);
                    }
                    else
                    {
                        throw new Exception("Yêu cầu rút tiền không tồn tại");
                    }
                }
            }
            else
            {
                throw new Exception("Tài khoản không tồn tại");
            }
        }

        public async Task<Pagination<TransactionModel>> GetTransationsWalletPaginationAsync(PaginationParameter paginationParameter, string email, TransactionFilter transactionFilter)
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
                        var transactions = await _unitOfWork.TransactionRepository.GetTransactionsWalletPaging(storeWallet.Id, paginationParameter, transactionFilter);
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

        public async Task<WithdrawModel> ProcessWithdrawStoreAsync(UpdateWithdrawModel updateWithdrawModel)
        {
            if (updateWithdrawModel.Status == WithdrawStatus.DONE)
            {
                var existWithdraw = await _unitOfWork.WithdrawRepository.GetByIdAsync(updateWithdrawModel.Id);
                if (existWithdraw != null && existWithdraw.Status.ToUpper() == WithdrawStatus.APPROVED.ToString())
                {
                    existWithdraw.Status = WithdrawStatus.DONE.ToString();
                    existWithdraw.TransferDate = CommonUtils.GetCurrentTime();
                    existWithdraw.ImageTransfer = updateWithdrawModel.ImageTransfer;

                    _unitOfWork.WithdrawRepository.UpdateAsync(existWithdraw);

                    // create transaction
                    var storeWallet = await _unitOfWork.WalletRepository.GetByIdAsync(existWithdraw.WalletId);
                    if (storeWallet == null)
                    {
                        throw new Exception("Ví không tồn tại");
                    }

                    storeWallet.Balance -= existWithdraw.Amount;
                    _unitOfWork.WalletRepository.UpdateAsync(storeWallet);

                    var storeInfo = await _unitOfWork.StoreRepository.GetStoreByIdAsync(storeWallet.StoreId.Value);
                    if (storeInfo == null)
                    {
                        throw new Exception("Cửa hàng không tồn tại");
                    }

                    var newTransaction = new Transaction
                    {
                        WalletId = existWithdraw.WalletId,
                        Status = TransactionStatus.SUCCESS.ToString(),
                        TransactionType = TransactionType.OUT.ToString(),
                        Amount = (int) existWithdraw.Amount,
                        Description = $"Rút tiền về tài khoản {storeInfo.AccountNumber} - {storeInfo.BankCode}",
                        TransactionDate = CommonUtils.GetCurrentTime()
                    };
                    await _unitOfWork.TransactionRepository.AddAsync(newTransaction);

                    _unitOfWork.Save();
                    return _mapper.Map<WithdrawModel>(existWithdraw);
                }
                else
                {
                    throw new Exception("Yêu cầu rút tiền không tồn tại");
                }
            }
            else
            {
                throw new Exception("Bạn chỉ có thể thực hiện giao dịch khi đã chấp nhận yêu cầu rút tiền");
            }
        }

        public async Task<WithdrawModel> RequestWithdrawStoreAsync(CreateWithdrawModel createWithdrawModel, string email)
        {
            // check user
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(currentUser.Id);
            if (store != null)
            {
                var storeWallet = await _unitOfWork.WalletRepository.GetWalletStoreAsync(store.Id);
                if (storeWallet != null)
                {
                    // check withdraw
                    if (storeWallet.Balance == 0)
                    {
                        throw new Exception("Không đủ số dư để thực hiện giao dịch");
                    }

                    if (storeWallet.Balance < createWithdrawModel.Amount)
                    {
                        throw new Exception($"Bạn chỉ có thể rút tối đa {storeWallet.Balance} VNĐ");
                    }

                    var newWithdraw = new Withdraw
                    {
                        WalletId = storeWallet.Id,
                        Amount = createWithdrawModel.Amount,
                        Requester = currentUser.Email
                    };

                    await _unitOfWork.WithdrawRepository.AddAsync(newWithdraw);

                    // noti


                    _unitOfWork.Save();

                    return _mapper.Map<WithdrawModel>(newWithdraw);
                }
                else
                {
                    throw new Exception("Cửa hàng không có ví");
                }
            }
            else
            {
                throw new Exception("Cửa hàng không tồn tại");
            }
        }
    }
}
