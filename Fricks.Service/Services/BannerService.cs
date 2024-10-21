using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.BannerModels;
using Fricks.Service.BusinessModel.CategoryModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class BannerService : IBannerService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BannerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BannerModel> AddBanner(BannerProcessModel banner)
        {
            var addBanner = _mapper.Map<Banner>(banner);
            var allBanner = await _unitOfWork.BannerRepository.GetAllAsync();
            foreach(var bannerExist in allBanner)
            {
                if(bannerExist.Index == banner.Index)
                {
                    throw new Exception("Index đã tồn tại");
                }
            }
            var result = await _unitOfWork.BannerRepository.AddAsync(addBanner);
            _unitOfWork.Save();
            return _mapper.Map<BannerModel>(result);
        }

        public async Task<BannerModel> DeleteBanner(int id)
        {
            var banner = await _unitOfWork.BannerRepository.GetByIdAsync(id);
            if(banner == null)
            {
                throw new Exception("Banner không tồn tại");   
            }
            _unitOfWork.BannerRepository.SoftDeleteAsync(banner);
            _unitOfWork.Save();
            return _mapper.Map<BannerModel>(banner);
        }

        public async Task<List<BannerModel>> GetAllBanner()
        {
            return _mapper.Map<List<BannerModel>>(await _unitOfWork.BannerRepository.GetAllAsync());
        }

        public async Task<BannerModel> GetBannerById(int id)
        {
            return _mapper.Map<BannerModel>(await _unitOfWork.BannerRepository.GetByIdAsync(id));
        }

        public async Task<BannerModel> UpdateBanner(BannerUpdateModel bannerModel)
        {
            var banner = await _unitOfWork.BannerRepository.GetByIdAsync(bannerModel.Id);
            if (banner == null)
            {
                throw new Exception("Banner không tồn tại");
            }
            var allBanner = await _unitOfWork.BannerRepository.GetAllAsync();
            foreach (var bannerExist in allBanner)
            {
                if (bannerExist.Index == bannerModel.Index)
                {
                    throw new Exception("Index đã tồn tại");
                }
            }
            var updateBanner = _mapper.Map(bannerModel, banner);
            _unitOfWork.BannerRepository.UpdateAsync(updateBanner);
            _unitOfWork.Save();
            return _mapper.Map<BannerModel>(updateBanner);
        }
    }
}
