using Fricks.Repository.Commons;
using Fricks.Service.BusinessModel.BannerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services.Interface
{
    public interface IBannerService
    {
        public Task<BannerModel> AddBanner(BannerProcessModel banner);
        public Task<BannerModel> UpdateBanner(int id, BannerProcessModel banner);
        public Task<BannerModel> DeleteBanner(int id);
        public Task<BannerModel> GetBannerById(int id);
        public Task<List<BannerModel>> GetAllBanner();
    }
}
