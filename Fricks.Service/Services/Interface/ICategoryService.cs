using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.CategoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface ICategoryService
    {
        public Task<CategoryModel> AddCategory(CategoryProcessModel Category);
        public Task<CategoryModel> UpdateCategory(int id, CategoryProcessModel Category);
        public Task<CategoryModel> DeleteCategory(int id);
        public Task<CategoryModel> GetCategoryById(int id);
        public Task<List<CategoryModel>> GetAllCategory();
        public Task<Pagination<CategoryModel>> GetAllCategoryPagination(PaginationParameter paginationParameter);
    }
}
