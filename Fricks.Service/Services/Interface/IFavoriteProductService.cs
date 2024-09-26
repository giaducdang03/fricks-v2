using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.FavoriteProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IFavoriteProductService
    {
        public Task<FavoriteProductModel> AddFavoriteProduct(FavoriteProductProcessModel FavoriteProduct);
        public Task<FavoriteProductModel> DeleteFavoriteProduct(int id);
        public Task<FavoriteProductModel> GetFavoriteProductById(int id);
        public Task<List<FavoriteProductModel>> GetAllFavoriteProduct();
        public Task<Pagination<FavoriteProductModel>> GetAllFavoriteProductPagination(PaginationParameter paginationParameter);
    }
}
