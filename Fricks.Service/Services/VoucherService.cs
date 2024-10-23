using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.VoucherModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class VoucherService : IVoucherService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<VoucherModel> AddVoucher(VoucherProcessModel voucherProcessModel, string email)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user == null) { throw new Exception("Không tìm thấy tài khoản."); }
            if (user.Role != RoleEnums.STORE.ToString()) { throw new Exception("Tài khoản không được phép tạo voucher."); }
            var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
            if (store == null) { throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng."); }
            var addVoucher = _mapper.Map<Voucher>(voucherProcessModel);
            var dupVoucher = await _unitOfWork.VoucherRepository.GetVoucherByCode(addVoucher.Code, store.Id);
            if (dupVoucher != null) { throw new Exception("Mã đã tồn tại trong cửa hàng của bạn."); }
            addVoucher.StoreId = store.Id;
            addVoucher.Status = VoucherStatus.Enable.ToString();
            var result = await _unitOfWork.VoucherRepository.AddAsync(addVoucher);
            _unitOfWork.Save();
            return _mapper.Map<VoucherModel>(result);
        }

        public async Task<VoucherModel> DeleteVoucher(int id, string email)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                throw new Exception("không tìm thấy voucher");
            }
            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user == null) { throw new Exception("Không tìm thấy tài khoản."); }
            var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
            if (store == null) { throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng."); }
            if (voucher.StoreId != store.Id) { throw new Exception("Voucher không thuộc sở hữu của cửa hàng."); }
            _unitOfWork.VoucherRepository.SoftDeleteAsync(voucher);
            _unitOfWork.Save();
            return _mapper.Map<VoucherModel>(voucher);
        }

        public async Task<VoucherModel> DisableVoucher(int id, string email)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                throw new Exception("không tìm thấy voucher");
            }
            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user == null) { throw new Exception("Không tìm thấy tài khoản."); }
            var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
            if (store == null) { throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng."); }
            if (voucher.StoreId != store.Id) { throw new Exception("Voucher không thuộc sở hữu của cửa hàng."); }
            voucher.Status = VoucherStatus.Disable.ToString();
            _unitOfWork.VoucherRepository.UpdateAsync(voucher);
            _unitOfWork.Save();
            return _mapper.Map<VoucherModel>(voucher);
        }

        public async Task<Pagination<VoucherModel>> GetAllVoucherByStoreId(int storeId, PaginationParameter paginationParameter)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(storeId);
            if (store == null) { throw new Exception("Không tìm thấy store."); }
            var result = await _unitOfWork.VoucherRepository.GetVoucherByStore(store.Id, paginationParameter);
            return _mapper.Map<Pagination<VoucherModel>>(result);
        }

        public async Task<VoucherModel> GetById(int id)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            return _mapper.Map<VoucherModel>(voucher);
        }

        public async Task<VoucherModel> UpdateVoucher(int id, VoucherProcessModel voucherProcessModel, string email)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                throw new Exception("không tìm thấy voucher");
            }
            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user == null) { throw new Exception("Không tìm thấy tài khoản."); }
            var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
            if (store == null) { throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng."); }
            if (voucher.StoreId != store.Id ) { throw new Exception("Voucher không thuộc sở hữu của cửa hàng."); }
            var updateVoucher = _mapper.Map(voucherProcessModel, voucher);
            _unitOfWork.VoucherRepository.UpdateAsync(updateVoucher);
            _unitOfWork.Save();
            return _mapper.Map<VoucherModel>(updateVoucher);
        }
    }
}
