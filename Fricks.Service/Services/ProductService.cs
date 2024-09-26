using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class ProductService : IProductService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProductModel> AddProduct(ProductRegisterModel product)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(product.BrandId);
            if (brand == null)
            {
                throw new Exception("Không tìm thấy hãng.");
            }
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
            {
                throw new Exception("Không tìm thấy danh mục sản phẩm.");
            }
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(product.StoreId);
            if (store == null)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }

            var addProduct = _mapper.Map<Product>(product);
            var result = await _unitOfWork.ProductRepository.AddAsync(addProduct);
            _unitOfWork.Save();
            return _mapper.Map<ProductModel>(result);
        }

        public async Task<ProductModel> DeleteProduct(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm - khổng thể xóa");
            }
            _unitOfWork.ProductRepository.SoftDeleteAsync(product);
            _unitOfWork.Save();
            return _mapper.Map<ProductModel>(product);
        }

        public async Task<Pagination<ProductModel>> GetAllProductByStoreIdPagination(int storeId, int brandId, int categoryId, PaginationParameter paginationParameter)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(brandId);
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
            var result = await _unitOfWork.ProductRepository.GetProductByStoreIdPaging(brand, category, storeId, paginationParameter);
            return _mapper.Map<Pagination<ProductModel>>(result);
        }

        public async Task<Pagination<ProductModel>> GetAllProductPagination(int brandId, int categoryId, PaginationParameter paginationParameter)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(brandId);
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
            var result = await _unitOfWork.ProductRepository.GetProductPaging(brand, category, paginationParameter);
            return _mapper.Map<Pagination<ProductModel>>(result);
        }

        public async Task<ProductModel> GetProductById(int id)
        {
            var result = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            return _mapper.Map<ProductModel>(result);
        }

        public async Task<ProductModel> UpdateProduct(int id, ProductProcessModel productModel)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm - không thể cập nhật.");
            }
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(productModel.BrandId);
            if (brand == null)
            {
                throw new Exception("Không tìm thấy hãng.");
            }
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(productModel.CategoryId);
            if (category == null)
            {
                throw new Exception("Không tìm thấy danh mục sản phẩm.");
            }
            var updateProduct = _mapper.Map(productModel, product);
            _unitOfWork.ProductRepository.UpdateAsync(updateProduct);
            _unitOfWork.Save();
            return _mapper.Map<ProductModel>(updateProduct);
        }
    }
}
