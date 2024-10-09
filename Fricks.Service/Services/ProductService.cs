using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Repository.Utils;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Utils;
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

        public async Task<ProductModel> AddProduct(CreateProductModel productModel, string email)
        {
            var userLogin = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (userLogin == null)
            {
                throw new Exception("Tải khoản không tồn tại");
            }

            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(productModel.BrandId);
            if (brand == null)
            {
                throw new Exception("Không tìm thấy thương hiệu");
            }

            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(productModel.CategoryId);
            if (category == null)
            {
                throw new Exception("Không tìm thấy danh mục sản phẩm");
            }

            // check store

            Store store;

            if (userLogin.Role.ToUpper() == RoleEnums.ADMIN.ToString())
            {
                store = await _unitOfWork.StoreRepository.GetByIdAsync(productModel.StoreId);
                if (store == null)
                {
                    throw new Exception("Cửa hàng không tồn tại");
                }
            }
            else
            {
                store = await _unitOfWork.StoreRepository.GetStoreByManagerId(userLogin.Id);
                if (store == null)
                {
                    throw new Exception("Tài khoản chưa quản lí cửa hàng nào");
                }
            }

            // check product
            var existProduct = await _unitOfWork.ProductRepository.GetProductBySKUAsync(productModel.Sku);
            if (existProduct != null)
            {
                throw new Exception("SKU không được trùng");
            }

            var newProduct = new Product
            {
                Sku = productModel.Sku,
                BrandId = productModel.BrandId,
                CategoryId = productModel.CategoryId,
                Description = productModel.Description,
                Image = productModel.Image,
                Quantity = productModel.Quantity,
                SoldQuantity = 0,
                Name = productModel.Name,
                UnsignName = StringUtils.ConvertToUnSign(productModel.Name),
                StoreId = store.Id,
                IsAvailable = true,
            };

            var validProductUnits = await _unitOfWork.ProductUnitRepository.GetAllAsync();

            // check valid product unit
            bool checkValidProductUnit = true;

            var listAddPrice = new List<ProductPrice>();
            foreach (var item in productModel.ProductPrices)
            {
                var productUnit = validProductUnits.FirstOrDefault(x => x.Code == item.UnitCode);
                if (productUnit == null)
                {
                    checkValidProductUnit = false;
                    break;
                }
            }

            //bool checkValidProductUnit = validProductUnits.All(item1 => productModel.ProductPrices.Any(item2 => item1.Code == item2.UnitCode));

            if (checkValidProductUnit)
            {
                var productPrice = new List<ProductPrice>();
                if (productModel.ProductPrices.Count > 0)
                {
                    foreach (var price in productModel.ProductPrices)
                    {
                        var unitId = validProductUnits.FirstOrDefault(x => x.Code == price.UnitCode).Id;
                        if (unitId != null)
                        {
                            var newPrice = new ProductPrice
                            {
                                UnitId = unitId,
                                Price = price.Price,
                                CreateDate = CommonUtils.GetCurrentTime()
                            };
                            productPrice.Add(newPrice);
                        }
                    }

                    newProduct.ProductPrices = productPrice;

                    await _unitOfWork.ProductRepository.AddAsync(newProduct);
                    _unitOfWork.Save();

                    return _mapper.Map<ProductModel>(newProduct);
                }
                else
                {
                    throw new Exception("Giá không được để trống");
                }

            }
            else
            {
                throw new Exception("Đơn vị tính (ĐVT) sản phẩm không hợp lệ");
            }

        }

        //public async Task<ProductModel> AddProduct(ProductRegisterModel product)
        //{
        //    var brand = await _unitOfWork.BrandRepository.GetByIdAsync(product.BrandId);
        //    if (brand == null)
        //    {
        //        throw new Exception("Không tìm thấy hãng.");
        //    }
        //    var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryId);
        //    if (category == null)
        //    {
        //        throw new Exception("Không tìm thấy danh mục sản phẩm.");
        //    }
        //    var store = await _unitOfWork.StoreRepository.GetByIdAsync(product.StoreId);
        //    if (store == null)
        //    {
        //        throw new Exception("Không tìm thấy cửa hàng.");
        //    }

        //    var addProduct = _mapper.Map<Product>(product);
        //    var result = await _unitOfWork.ProductRepository.AddAsync(addProduct);
        //    _unitOfWork.Save();
        //    return _mapper.Map<ProductModel>(result);
        //}

        public async Task<ProductModel> DeleteProduct(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null || product.IsDeleted == true)
            {
                throw new Exception("Sản phẩm không tồn tại");
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

        public async Task<Pagination<ProductListModel>> GetAllProductPagination(PaginationParameter paginationParameter, ProductFilter productFilter, string currentEmail)
        {
            if (currentEmail == null)
            {
                var result = await _unitOfWork.ProductRepository.GetProductPagingAsync(paginationParameter, productFilter);
                return _mapper.Map<Pagination<ProductListModel>>(result);
            }
            else
            {
                var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(currentEmail);
                if (currentUser == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                if (currentUser.Role.ToUpper() == RoleEnums.STORE.ToString().ToUpper())
                {
                    var currentStore = await _unitOfWork.StoreRepository.GetStoreByManagerId(currentUser.Id);
                    if (currentStore == null)
                    {
                        throw new Exception("Tài khoản chưa quản lí cửa hàng nào");
                    }
                    productFilter.StoreId = currentStore.Id;
                    var result = await _unitOfWork.ProductRepository.GetProductPagingAsync(paginationParameter, productFilter);
                    return _mapper.Map<Pagination<ProductListModel>>(result);
                }
                else
                {
                    var result = await _unitOfWork.ProductRepository.GetProductPagingAsync(paginationParameter, productFilter);
                    return _mapper.Map<Pagination<ProductListModel>>(result);
                }
            }
        }

        public async Task<ProductModel> GetProductById(int id)
        {
            var result = await _unitOfWork.ProductRepository.GetProductByIdAsync(id);
            return _mapper.Map<ProductModel>(result);
        }

        public async Task<ProductModel> UpdateProduct(int id, ProductProcessModel productModel)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại");
            }
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(productModel.BrandId);
            if (brand == null)
            {
                throw new Exception("Không tìm thấy hãng");
            }
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(productModel.CategoryId);
            if (category == null)
            {
                throw new Exception("Không tìm thấy danh mục sản phẩm");
            }
            var updateProduct = _mapper.Map(productModel, product);
            _unitOfWork.ProductRepository.UpdateProductAsync(updateProduct);
            _unitOfWork.Save();
            return _mapper.Map<ProductModel>(updateProduct);
        }
    }
}
