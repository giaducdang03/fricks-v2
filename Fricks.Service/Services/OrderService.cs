using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Utils;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Services
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork _UnitOfWork;
        private IMapper _mapper;
        private IOrderDetailService _orderDetailService;
        private IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderDetailService orderDetailService, IPaymentService paymentService)
        {
            _UnitOfWork = unitOfWork;
            _mapper = mapper;
            _orderDetailService = orderDetailService;
            _paymentService = paymentService;
        }

        public async Task<CreatePaymentResult> AddOrder(OrderProcessModel orderProcessModel)
        {
            //Validate Order
            // ...
            //Validate OrderDetail
            // ...
            //Này là flow bình thường, không có khách hàng thân thiết
            var addOrder = _mapper.Map<Order>(orderProcessModel);
            addOrder.Status = true.ToString();
            addOrder.PaymentStatus = PaymentStatus.PENDING.ToString();
            addOrder.OrderCode = NumberUtils.GetRandomLong();
            await _UnitOfWork.OrderRepository.AddAsync(addOrder); //Nó add luôn orderDetail nên ko cần gọi orderDetail
            _UnitOfWork.Save();
            //Nếu tạo được order => tạo link thanh toán
            return await _paymentService.CreatePaymentLinkOrder(orderProcessModel.Total, addOrder);
        }

        public Task<Pagination<OrderModel>> GetOrderByStoreId(int storeId)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<OrderModel>> GetOrderByUserEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
