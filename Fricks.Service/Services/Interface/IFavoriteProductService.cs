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
        public Task<FavoriteProductModelAdd> AddFavoriteProduct(string email, FavoriteProductProcessModel FavoriteProduct);
        public Task<FavoriteProductModel> DeleteFavoriteProduct(int id);
        public Task<bool> DeleteAllUserFavoriteProduct(string email);
        public Task<List<FavoriteProductModel>> GetAllFavoriteProduct();
        public Task<Pagination<FavoriteProductModel>> GetUserFavoriteProductsPagination(string email, PaginationParameter paginationParameter);
    }
}
