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
            if (user == null) {
                throw new Exception("Không tìm thấy tài khoản.");
            }

            // create voucher with admin role
            if (user.Role == RoleEnums.ADMIN.ToString()) 
            {
                // check availability of global voucher
                if (voucherProcessModel.Availability != AvailabilityVoucher.GLOBAL)
                {
                    throw new Exception("Tài khoản không được phép tạo voucher với phạm vi cửa hàng.");
                }

                var addVoucher = _mapper.Map<Voucher>(voucherProcessModel);

                var dupVoucher = await _unitOfWork.VoucherRepository.GetGlobalVoucherByCode(addVoucher.Code);
                if (dupVoucher != null)
                {
                    throw new Exception("Mã đã tồn tại.");
                }

                addVoucher.Status = VoucherStatus.ENABLE.ToString();
                var result = await _unitOfWork.VoucherRepository.AddAsync(addVoucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(result);
            } 
            else if (user.Role == RoleEnums.STORE.ToString())
            {
                // check availability of store
                if (voucherProcessModel.Availability != AvailabilityVoucher.STORE)
                {
                    throw new Exception("Tài khoản không được phép tạo voucher với phạm vi toàn bộ.");
                }

                var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
                if (store == null)
                {
                    throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng.");
                }
                var addVoucher = _mapper.Map<Voucher>(voucherProcessModel);
                var dupVoucher = await _unitOfWork.VoucherRepository.GetVoucherByCode(addVoucher.Code, store.Id);
                if (dupVoucher != null)
                {
                    throw new Exception("Mã đã tồn tại trong cửa hàng của bạn.");
                }
                addVoucher.StoreId = store.Id;
                addVoucher.Status = VoucherStatus.ENABLE.ToString();
                var result = await _unitOfWork.VoucherRepository.AddAsync(addVoucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(result);
            } 
            else 
            {
                throw new Exception("Tài khoản không được phép tạo voucher.");
            }
        }

        public async Task<VoucherModel> DeleteVoucher(int id, string email)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                throw new Exception("không tìm thấy voucher");
            }

            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("Không tìm thấy tài khoản.");
            }

            if (user.Role == RoleEnums.ADMIN.ToString())
            {
                // Chỉ cho phép xóa voucher global
                if (voucher.StoreId != null)
                {
                    throw new Exception("Tài khoản admin chỉ được phép xóa voucher toàn bộ.");
                }
                _unitOfWork.VoucherRepository.SoftDeleteAsync(voucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(voucher);
            }
            else if (user.Role == RoleEnums.STORE.ToString())
            {
                var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
                if (store == null)
                {
                    throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng.");
                }
                if (voucher.StoreId != store.Id)
                {
                    throw new Exception("Voucher không thuộc sở hữu của cửa hàng.");
                }
                _unitOfWork.VoucherRepository.SoftDeleteAsync(voucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(voucher);
            }
            else
            {
                throw new Exception("Tài khoản không được phép xóa voucher.");
            }
        }

        public async Task<VoucherModel> DisableVoucher(int id, string email)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                throw new Exception("không tìm thấy voucher");
            }

            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("Không tìm thấy tài khoản.");
            }

            if (user.Role == RoleEnums.ADMIN.ToString())
            {
                // Chỉ cho phép disable voucher global
                if (voucher.StoreId != null)
                {
                    throw new Exception("Tài khoản admin chỉ được phép disable voucher toàn bộ.");
                }
                voucher.Status = VoucherStatus.DISABLE.ToString();
                _unitOfWork.VoucherRepository.UpdateAsync(voucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(voucher);
            }
            else if (user.Role == RoleEnums.STORE.ToString())
            {
                var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
                if (store == null)
                {
                    throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng.");
                }
                if (voucher.StoreId != store.Id)
                {
                    throw new Exception("Voucher không thuộc sở hữu của cửa hàng.");
                }
                voucher.Status = VoucherStatus.DISABLE.ToString();
                _unitOfWork.VoucherRepository.UpdateAsync(voucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(voucher);
            }
            else
            {
                throw new Exception("Tài khoản không được phép disable voucher.");
            }
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

        public async Task<Pagination<VoucherModel>> GetVouchersPaging(PaginationParameter paginationParameter)
        {
            var result = await _unitOfWork.VoucherRepository.GetVouchersAsync(paginationParameter);
            return _mapper.Map<Pagination<VoucherModel>>(result);
        }

        public async Task<VoucherModel> UpdateVoucher(int id, VoucherProcessModel voucherProcessModel, string email)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                throw new Exception("không tìm thấy voucher");
            }

            var user = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("Không tìm thấy tài khoản.");
            }

            if (user.Role == RoleEnums.ADMIN.ToString())
            {
                // Chỉ cho phép cập nhật voucher global
                if (voucherProcessModel.Availability != AvailabilityVoucher.GLOBAL)
                {
                    throw new Exception("Tài khoản không được phép cập nhật voucher với phạm vi cửa hàng.");
                }

                // Kiểm tra trùng mã (trừ chính voucher đang cập nhật)
                var dupVoucher = await _unitOfWork.VoucherRepository.GetGlobalVoucherByCode(voucherProcessModel.Code);
                if (dupVoucher != null && dupVoucher.Id != voucher.Id)
                {
                    throw new Exception("Mã đã tồn tại.");
                }

                var updateVoucher = _mapper.Map(voucherProcessModel, voucher);
                updateVoucher.Status = VoucherStatus.ENABLE.ToString();
                _unitOfWork.VoucherRepository.UpdateAsync(updateVoucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(updateVoucher);
            }
            else if (user.Role == RoleEnums.STORE.ToString())
            {
                // Chỉ cho phép cập nhật voucher store
                if (voucherProcessModel.Availability != AvailabilityVoucher.STORE)
                {
                    throw new Exception("Tài khoản không được phép cập nhật voucher với phạm vi toàn bộ.");
                }

                var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(user.Id);
                if (store == null)
                {
                    throw new Exception($"Tài khoản {user.Id} không sở hữu cửa hàng.");
                }
                if (voucher.StoreId != store.Id)
                {
                    throw new Exception("Voucher không thuộc sở hữu của cửa hàng.");
                }

                // Kiểm tra trùng mã (trừ chính voucher đang cập nhật)
                var dupVoucher = await _unitOfWork.VoucherRepository.GetVoucherByCode(voucherProcessModel.Code, store.Id);
                if (dupVoucher != null && dupVoucher.Id != voucher.Id)
                {
                    throw new Exception("Mã đã tồn tại trong cửa hàng của bạn.");
                }

                var updateVoucher = _mapper.Map(voucherProcessModel, voucher);
                updateVoucher.StoreId = store.Id;
                updateVoucher.Status = VoucherStatus.ENABLE.ToString();
                _unitOfWork.VoucherRepository.UpdateAsync(updateVoucher);
                _unitOfWork.Save();
                return _mapper.Map<VoucherModel>(updateVoucher);
            }
            else
            {
                throw new Exception("Tài khoản không được phép cập nhật voucher.");
            }
        }
    }
}
