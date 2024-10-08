using AutoMapper;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.OrderDetailModels;
using Fricks.Service.Services.Interface;
using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

namespace Fricks.Service.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<OrderDetailProcessModel>> CreateOrderDetail(List<OrderDetailProcessModel> orderDetail)
        {
            var addOrderDetail = _mapper.Map<List<OrderDetail>>(orderDetail);
            await _unitOfWork.OrderDetailRepository.AddRangeAsync(addOrderDetail);
            _unitOfWork.Save();
            return orderDetail;
        }

        public Task<List<OrderDetailModel>> GetByOrderId(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
