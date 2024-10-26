using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.ProductPriceModels;
using Fricks.Service.BusinessModel.ProductUnitModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class ProductPriceService : IProductPriceService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ProductPriceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductPriceModel> AddProductPrice(ProductPriceRegisterModel model)
        {
            // get product unit
            var validProductUnits = await _unitOfWork.ProductUnitRepository.GetAllAsync();
            var addProductPriceUnit = validProductUnits.FirstOrDefault(x => x.Code == model.UnitCode);
            if (addProductPriceUnit == null)
            {
                throw new Exception("Đơn vị tính không hợp lệ");
            }
            var newProductPrice = new ProductPrice
            {
                UnitId = addProductPriceUnit.Id,
                Price = model.Price,
            };
            var result = await _unitOfWork.ProductPriceRepository.AddAsync(newProductPrice);
            _unitOfWork.Save();
            return _mapper.Map<ProductPriceModel>(result);
        }

        public async Task<ProductPriceModel> DeleteProductPrice(int id)
        {
            var productPrice = await _unitOfWork.ProductPriceRepository.GetByIdAsync(id);
            if (productPrice == null)
            {
                throw new Exception("Không tìm thấy gía - Không thể xóa");
            }
            _unitOfWork.ProductPriceRepository.SoftDeleteAsync(productPrice);
            _unitOfWork.Save();
            return _mapper.Map<ProductPriceModel>(productPrice);
        }

        public async Task<List<ProductPriceModel>> GetAllProductPrice()
        {
            var result = await _unitOfWork.ProductPriceRepository.GetAllAsync();
            return _mapper.Map<List<ProductPriceModel>>(result);
        }

        public async Task<ProductPriceModel> GetProductPriceById(int id)
        {
            var result = await _unitOfWork.ProductPriceRepository.GetByIdAsync(id);
            return _mapper.Map<ProductPriceModel>(result);
        }

        public async Task<List<ProductPriceModel>> GetProductPriceByProductId(int id)
        {
            var result = await _unitOfWork.ProductPriceRepository.GetByProductId(id);
            return _mapper.Map<List<ProductPriceModel>>(result);
        }

        public async Task<ProductPriceModel> UpdateProductPrice(ProductPriceProcessModel model)
        {
            var productPrice = await _unitOfWork.ProductPriceRepository.GetByIdAsync(model.Id);
            if (productPrice == null)
            {
                throw new Exception("Không tìm thấy giá - Không thể cập nhật");
            }
            var updatePrice = _mapper.Map(model, productPrice);
            _unitOfWork.ProductPriceRepository.UpdateAsync(updatePrice);
            _unitOfWork.Save();
            return _mapper.Map<ProductPriceModel>(updatePrice);
        }
    }
}
