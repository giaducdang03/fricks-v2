using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.BusinessModel.StoreModels;
using Fricks.Service.Services.Interface;
using MailKit.Net.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class StoreService : IStoreService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public StoreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StoreModel> AddStore(StoreRegisterModel store)
        {
            // check manager
            var existStore = await _unitOfWork.StoreRepository.GetStoreByManagerId(store.ManagerId);
            if (existStore != null)
            {
                throw new Exception("Tài khoản này đã có cửa hàng vui lòng chọn tài khoản khác");
            }

            var addStore = _mapper.Map<Store>(store);
            var result = await _unitOfWork.StoreRepository.AddAsync(addStore);
            _unitOfWork.Save();
            return _mapper.Map<StoreModel>(result);
        }

        public async Task<StoreModel> DeleteStore(int id)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            if (store == null)
            {
                throw new Exception("Không tìm thấy cửa hàng - Không thể xóa");
            }
            _unitOfWork.StoreRepository.SoftDeleteAsync(store);
            _unitOfWork.Save();
            return _mapper.Map<StoreModel>(store);
        }

        public async Task<List<StoreModel>> GetAllStore()
        {
            var result = await _unitOfWork.StoreRepository.GetAllAsync();
            return _mapper.Map<List<StoreModel>>(result);
        }

        public async Task<Pagination<StoreModel>> GetAllStorePagination(PaginationParameter paginationParameter)
        {
            var result = await _unitOfWork.StoreRepository.GetStorePaging(paginationParameter);
            return _mapper.Map<Pagination<StoreModel>>(result);
        }

        public async Task<StoreModel> GetStoreById(int id)
        {
            var result = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            return _mapper.Map<StoreModel>(result);
        }

        public async Task<Pagination<StoreModel>> GetStoreByManagerId(PaginationParameter paginationParameter, int id)
        {
            var result = await _unitOfWork.StoreRepository.GetStoreByManagerIdPaging(paginationParameter, id);
            return _mapper.Map<Pagination<StoreModel>>(result);
        }

        public async Task<StoreModel> UpdateStore(int id, StoreProcessModel storeModel)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            if (store == null)
            {
                throw new Exception("Không tìm thấy cửa hàng - Không thể cập nhật");
            }
            var updateStore = _mapper.Map(storeModel, store);
            _unitOfWork.StoreRepository.UpdateAsync(updateStore);
            _unitOfWork.Save();
            return _mapper.Map<StoreModel>(updateStore);
        }
    }
}
