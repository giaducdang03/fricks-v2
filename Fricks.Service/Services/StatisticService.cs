using AutoMapper;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Repository.Utils;
using Fricks.Service.BusinessModel.DashboardModels;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CategoryRevenueModel>> GetCategoryRevenueAdminAsync(int month, int year)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllOrderDetails();

            var result = categories.Select(category => new CategoryRevenueModel
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Revenue = orderDetails
                .Where(order => order.Order.PaymentStatus == PaymentStatus.PAID.ToString() && order.CreateDate.Year == year && order.CreateDate.Month == month && order.Product.CategoryId == category.Id)
                .Sum(order => order.Price.Value * order.Quantity.Value),
                        LastUpdated = CommonUtils.GetCurrentTime()
            }).ToList();

            return result;
        }

        public async Task<CommonAdminInfoModel> GetCommonInfoAdminAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            decimal totalRevenue = orders.Where(order => order.PaymentStatus == PaymentStatus.PAID.ToString()).Sum(order => order.Total.Value);

            var stores = await _unitOfWork.StoreRepository.GetAllAsync();
            int numOfStores = stores.Count();

            var users = await _unitOfWork.UsersRepository.GetAllAsync();
            int numOfUsers = users.Count();

            var products = await _unitOfWork.ProductRepository.GetAllAsync();
            int numOfProducts = products.Count();

            return new CommonAdminInfoModel
            {
                LastUpdated = CommonUtils.GetCurrentTime(),
                NumOfProducts = numOfProducts,
                NumOfStores = numOfStores,
                NumOfUsers = numOfUsers,
                Revenue = totalRevenue,
            };
        }

        public async Task<List<ProductListModel>> GetFeaturedProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllProductsAsync();
            return _mapper.Map<List<ProductListModel>>(products.OrderByDescending(x => x.SoldQuantity).Take(10));
        }

        public async Task<List<MainChartAdminModel>> GetMainChartAdminInfoAsync()
        {
            // Get the current date and the start of the week
            var dateNow = CommonUtils.GetCurrentTime();
            var startOfWeek = dateNow.AddDays(-(int)dateNow.DayOfWeek);

            var orders = await _unitOfWork.OrderRepository.GetAllAsync();

            var result = orders
                .Where(order => order.PaymentStatus == PaymentStatus.PAID.ToString() &&
                                order.CreateDate >= startOfWeek && order.CreateDate <= dateNow)
                .GroupBy(order => order.CreateDate.Date)
                .Select(group => new MainChartAdminModel
                {
                    Date = group.Key,
                    OrderCount = group.Count(),
                    Revenue = group.Sum(order => order.Total.Value)
                }).ToList();

            return result;
        }

        public async Task<List<StoreRevenueModel>> GetStoreRevenueModelAdminAsync(int month, int year)
        {
            var stores = await _unitOfWork.StoreRepository.GetAllAsync();
            var orders = await _unitOfWork.OrderRepository.GetAllOrderAsync();

            var result = stores.Select(store => new StoreRevenueModel
            {
                StoreId = store.Id,
                StoreName = store.Name,
                Revenue = orders
                    .Where(order => order.PaymentStatus == PaymentStatus.PAID.ToString() && order.CreateDate.Year == year && order.CreateDate.Month == month && order.StoreId == store.Id)
                    .Sum(order => order.Total.Value),
                LastUpdated = CommonUtils.GetCurrentTime()
            }).ToList();

            return result.Any() ? result : new List<StoreRevenueModel>();
        }

        public async Task<CommonStoreInfoModel> GetCommonInfoStoreAsync(string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            var currentStore = await _unitOfWork.StoreRepository.GetStoreByManagerId(currentUser.Id);
            if (currentUser == null)
            {
                throw new Exception("Người dùng chưa quản lí cửa hàng hoặc cửa hàng không tồn tại");
            }

            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            orders = orders.Where(x => x.StoreId == currentStore.Id && x.PaymentStatus == PaymentStatus.PAID.ToString()).ToList();
            decimal totalRevenue = orders.Sum(order => order.Total.Value);

            var products = await _unitOfWork.ProductRepository.GetAllAsync();
            int numOfProducts = products.Where(x => x.StoreId == currentStore.Id).Count();

            return new CommonStoreInfoModel
            {
                LastUpdated = CommonUtils.GetCurrentTime(),
                NumOfProducts = numOfProducts,
                NumOfOrders = orders.Count,
                Revenue = totalRevenue,
            };
        }
    }
}
