using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.BrandModels;
using Fricks.Service.BusinessModel.CategoryModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryModel> AddCategory(CategoryProcessModel category)
        {
            var addCategory = _mapper.Map<Category>(category);
            var result = await _unitOfWork.CategoryRepository.AddAsync(addCategory);
            _unitOfWork.Save();
            return _mapper.Map<CategoryModel>(result);
        }

        public async Task<CategoryModel> DeleteCategory(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            //Lấy danh sách các product đang có xem có đang trong danh mục sản phẩm này ko
            //...
            //
            if (category == null)
            {
                throw new Exception("Không tìm thấy danh mục sản phẩm - Không thể xóa");
            }
            _unitOfWork.CategoryRepository.SoftDeleteAsync(category);
            _unitOfWork.Save();
            return _mapper.Map<CategoryModel>(category);
        }

        public async Task<List<CategoryModel>> GetAllCategory()
        {
            var listCategory = await _unitOfWork.CategoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryModel>>(listCategory);
        }

        public async Task<Pagination<CategoryModel>> GetAllCategoryPagination(PaginationParameter paginationParameter)
        {
            var listCategory = await _unitOfWork.CategoryRepository.GetCategoryPaging(paginationParameter);
            return _mapper.Map<Pagination<CategoryModel>>(listCategory);
        }

        public async Task<CategoryModel> GetCategoryById(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoryModel>(category);
        }

        public async Task<CategoryModel> UpdateCategory(int id, CategoryProcessModel categoryProcessModel)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new Exception("Không tìm thấy danh mục sản phẩm - Không thể cập nhật");
            }
            var updateCategory = _mapper.Map(categoryProcessModel, category);
            _unitOfWork.CategoryRepository.UpdateAsync(updateCategory);
            _unitOfWork.Save();
            return _mapper.Map<CategoryModel>(updateCategory);
        }
    }
}
