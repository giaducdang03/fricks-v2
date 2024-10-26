using Fricks.Service.BusinessModel.DashboardModels;
using Fricks.Service.BusinessModel.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IStatisticService
    {
        public Task<CommonAdminInfoModel> GetCommonInfoAdminAsync();

        public Task<List<MainChartAdminModel>> GetMainChartAdminInfoAsync();

        public Task<List<CategoryRevenueModel>> GetCategoryRevenueAdminAsync(int month, int year);

        public Task<List<StoreRevenueModel>> GetStoreRevenueModelAdminAsync(int month, int year);

        public Task<List<ProductListModel>> GetFeaturedProductsAsync();

        // store
        public Task<CommonStoreInfoModel> GetCommonInfoStoreAsync(string email);
    }
}
