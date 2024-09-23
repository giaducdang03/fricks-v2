using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class BrandService : IBrandService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BrandModel> AddBrand(BrandProcessModel brand)
        {
            var addBrand = _mapper.Map<Brand>(brand);
            var result = await _unitOfWork.BrandRepository.AddAsync(addBrand);
            _unitOfWork.Save();
            return _mapper.Map<BrandModel>(result);
        }

        public async Task<BrandModel> DeleteBrand(int id)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            //Lấy danh sách các product đang có xem có đang của hãng này ko
            //...
            //
            if (brand == null)
            {
                throw new Exception("Không tìm thấy hãng - Không thể xóa");
            }
            _unitOfWork.BrandRepository.SoftDeleteAsync(brand);
            _unitOfWork.Save();
            return _mapper.Map<BrandModel>(brand);
        }

        public async Task<List<BrandModel>> GetAllBrand()
        {
            var listBrand = await _unitOfWork.BrandRepository.GetAllAsync();
            return _mapper.Map<List<BrandModel>>(listBrand);
        }

        public async Task<Pagination<BrandModel>> GetAllBrandPagination(PaginationParameter paginationParameter)
        {
            var listBrand = await _unitOfWork.BrandRepository.GetBrandPaging(paginationParameter);
            return _mapper.Map<Pagination<BrandModel>>(listBrand);
        }

        public async Task<BrandModel> GetBrandById(int id)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            return _mapper.Map<BrandModel>(brand);
        }

        public async Task<BrandModel> UpdateBrand(int id, BrandProcessModel brandModel)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                throw new Exception("Không tìm thấy hãng - Không thể cập nhật");
            }
            var updateBrand = _mapper.Map(brandModel, brand);
            _unitOfWork.BrandRepository.UpdateAsync(updateBrand);
            _unitOfWork.Save();
            return _mapper.Map<BrandModel>(updateBrand);
        }
    }
}
