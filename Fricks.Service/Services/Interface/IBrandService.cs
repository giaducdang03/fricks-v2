using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.BrandModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IBrandService
    {
        public Task<BrandModel> AddBrand(BrandProcessModel brand);
        public Task<BrandModel> UpdateBrand(int id, BrandProcessModel brand);
        public Task<BrandModel> DeleteBrand(int id);
        public Task<BrandModel> GetBrandById(int id);
        public Task<List<BrandModel>> GetAllBrand();
        public Task<Pagination<BrandModel>> GetAllBrandPagination(PaginationParameter paginationParameter);
    }
}
