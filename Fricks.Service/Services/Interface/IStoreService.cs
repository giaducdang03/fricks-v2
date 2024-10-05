using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.StoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IStoreService
    {
        public Task<StoreModel> AddStore(StoreRegisterModel Store);
        public Task<StoreModel> UpdateStore(int id, StoreProcessModel Store);
        public Task<StoreModel> DeleteStore(int id);
        public Task<StoreModel> GetStoreById(int id);
        //public Task<Pagination<StoreModel>> GetStoreByManagerId(PaginationParameter paginationParameter, int id);
        public Task<StoreModel> GetStoreByManagerId(int managerId);
        public Task<List<StoreModel>> GetAllStore();
        public Task<Pagination<StoreModel>> GetAllStorePagination(PaginationParameter paginationParameter);
    }
}
