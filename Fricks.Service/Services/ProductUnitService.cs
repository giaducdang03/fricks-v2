using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.ProductUnitModels;
using Fricks.Service.BusinessModel.StoreModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class ProductUnitService : IProductUnitService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ProductUnitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProductUnitModel> AddProductUnit(ProductUnitProcessModel productUnit)
        {
            var addProductUnit = _mapper.Map<ProductUnit>(productUnit);
            var result = await _unitOfWork.ProductUnitRepository.AddAsync(addProductUnit);
            _unitOfWork.Save();
            return _mapper.Map<ProductUnitModel>(result);
        }

        public async Task<ProductUnitModel> DeleteProductUnit(int id)
        {
            var productUnit = await _unitOfWork.ProductUnitRepository.GetByIdAsync(id);
            if (productUnit == null)
            {
                throw new Exception("Không tìm thấy đơn vị - Không thể xóa");
            }
            _unitOfWork.ProductUnitRepository.SoftDeleteAsync(productUnit);
            _unitOfWork.Save();
            return _mapper.Map<ProductUnitModel>(productUnit);
        }

        public async Task<List<ProductUnitModel>> GetAllProductUnit()
        {
            var result = await _unitOfWork.ProductUnitRepository.GetAllAsync();
            return _mapper.Map<List<ProductUnitModel>>(result); 
        }

        public async Task<ProductUnitModel> GetProductUnitById(int id)
        {
            var result = await _unitOfWork.ProductUnitRepository.GetByIdAsync(id);
            return _mapper.Map<ProductUnitModel>(result);
        }

        public async Task<ProductUnitModel> UpdateProductUnit(int id, ProductUnitProcessModel model)
        {
            var productUnit = await _unitOfWork.ProductUnitRepository.GetByIdAsync(id);
            if (productUnit == null)
            {
                throw new Exception("Không tìm thấy cửa hàng - Không thể cập nhật");
            }
            var updateUnit = _mapper.Map(model, productUnit);
            _unitOfWork.ProductUnitRepository.UpdateAsync(updateUnit);
            _unitOfWork.Save();
            return _mapper.Map<ProductUnitModel>(updateUnit);
        }
    }
}
