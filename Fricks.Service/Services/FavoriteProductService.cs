using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.CategoryModels;
using Fricks.Service.BusinessModel.FavoriteProductModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class FavoriteProductService : IFavoriteProductService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public FavoriteProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FavoriteProductModel> AddFavoriteProduct(FavoriteProductProcessModel favoriteProduct)
        {
            var addFavProduct = _mapper.Map<FavoriteProduct>(favoriteProduct);
            var result = await _unitOfWork.FavoriteProductRepository.AddAsync(addFavProduct);
            _unitOfWork.Save();
            return _mapper.Map<FavoriteProductModel>(result);
        }

        public async Task<FavoriteProductModel> DeleteFavoriteProduct(int id)
        {
            var favProduct = await _unitOfWork.FavoriteProductRepository.GetByIdAsync(id);
            if (favProduct == null)
            {
                throw new Exception("Không tìm thấy sản phẩm yêu thích - Không thể xóa");
            }
            _unitOfWork.FavoriteProductRepository.PermanentDeletedAsync(favProduct);
            _unitOfWork.Save();
            return _mapper.Map<FavoriteProductModel>(favProduct);
        }

        public async Task<List<FavoriteProductModel>> GetAllFavoriteProduct()
        {
            var listFavProduct = await _unitOfWork.FavoriteProductRepository.GetAllAsync();
            return _mapper.Map<List<FavoriteProductModel>>(listFavProduct);
        }

        public async Task<Pagination<FavoriteProductModel>> GetAllFavoriteProductPagination(int userId, PaginationParameter paginationParameter)
        {
            var listFavProduct = await _unitOfWork.FavoriteProductRepository.GetFavoriteProductPaging(userId, paginationParameter);
            return _mapper.Map<Pagination<FavoriteProductModel>>(listFavProduct);
        }
    }
}
