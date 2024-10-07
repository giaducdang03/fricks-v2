using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Repository.Utils;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.BusinessModel.ProductModels;
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
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IOrderDetailService _orderDetailService;
        private IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderDetailService orderDetailService, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
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
            addOrder.Code = NumberUtils.GetRandomLong().ToString();
            await _unitOfWork.OrderRepository.AddAsync(addOrder); //Nó add luôn orderDetail nên ko cần gọi orderDetail
            _unitOfWork.Save();
            //Nếu tạo được order => tạo link thanh toán
            return await _paymentService.CreatePaymentLinkOrder(orderProcessModel.Total, addOrder);
        }

        public async Task<OrderModel> CreateOrderAsync(CreateOrderModel orderModel)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(orderModel.CustomerEmail);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var dbProduct = await _unitOfWork.ProductRepository.GetAllProductsAsync();
            var validProductUnits = await _unitOfWork.ProductUnitRepository.GetAllAsync();

            if (orderModel.ProductOrders.Count > 0)
            {
                // check list product
                bool isProductValid = true;
                foreach (var item in orderModel.ProductOrders)
                {
                    var productValid = dbProduct.FirstOrDefault(x => x.Id == item.ProductId);
                    var unitValid = validProductUnits.FirstOrDefault(x => x.Id == item.ProductUnitId);
                    if (productValid == null || unitValid == null)
                    {
                        isProductValid = false;
                        break;
                    }
                }

                if (!isProductValid)
                {
                    throw new Exception("Sản phẩm hoặc đơn vị tính không tồn tại");
                }

                var orderedProducts = dbProduct.Where(p => orderModel.ProductOrders.Any(o => o.ProductId == p.Id));

                // check store
                bool isOneStore = orderedProducts.All(p => p.StoreId == orderedProducts.First().StoreId); ;
                if (!isOneStore)
                {
                    throw new Exception("Mỗi đơn hàng chỉ có thể mua sản phẩm của cùng một cửa hàng");
                }

                int storeId = orderedProducts.First().StoreId.Value;

                int totalPrice = 0;

                List<OrderDetail> orderdetails = new List<OrderDetail>();
                foreach (var order in orderedProducts)
                {
                    // get price unit
                    var temp = orderModel.ProductOrders.FirstOrDefault(x => x.ProductId == order.Id);
                    if (temp != null)
                    {
                        var temp2 = order.ProductPrices.FirstOrDefault(x => x.Unit.Id == temp.ProductUnitId);
                        if (temp2 != null)
                        {
                            totalPrice += temp2.Price.Value * temp.Quantity;
                            var newOrderDetail = new OrderDetail
                            {
                                ProductId = temp2.ProductId,
                                Price = temp2.Price.Value,
                                Quantity = temp.Quantity,
                                CreateDate = CommonUtils.GetCurrentTime(),
                            };
                            orderdetails.Add(newOrderDetail);
                        }
                    }
                }

                var newOrder = new Order
                {
                    UserId = currentUser.Id,
                    CustomerEmail = currentUser.Email,
                    CustomerName = currentUser.FullName,
                    CustomerPhone = currentUser.PhoneNumber != null ? currentUser.PhoneNumber : orderModel.CustomerPhone,
                    CustomerAddress = currentUser.Address != null ? currentUser.Address : orderModel.CustomerAddress,
                    Status = OrderStatus.PENDING.ToString(),
                    PaymentMethod = orderModel.PaymentMethod.ToString(),
                    PaymentStatus = PaymentStatus.PENDING.ToString(),
                    Code = GenerateOrderCode(storeId),
                    StoreId = storeId,
                    Total = totalPrice,
                    OrderDetails = orderdetails
                };

                await _unitOfWork.OrderRepository.AddAsync(newOrder);
                _unitOfWork.Save();

                return _mapper.Map<OrderModel>(newOrder);
            }
            throw new Exception("Không có sản phẩm nào trong danh sách đặt hàng");
        }

        public Task<Pagination<OrderModel>> GetOrderByStoreId(int storeId)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<OrderModel>> GetOrderByUserEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<CreatePaymentResult> ConfirmOrderAsync(ConfirmOrderModel orderModel, string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderModel.OrderId);
            if (order != null && order.UserId == currentUser.Id)
            {
                if (order.Status == OrderStatus.PENDING.ToString() 
                    &&  order.PaymentStatus == PaymentStatus.PENDING.ToString())
                {
                    var storeWallet = await _unitOfWork.WalletRepository.GetWalletStoreAsync(order.StoreId.Value);
                    if (storeWallet == null)
                    {
                        throw new Exception("Không tìm thấy ví của cửa hàng");
                    }

                    // calculate commission
                    // default 5% on order
                    double commission = order.Total.Value * 0.05;
                    double transactionAmount = order.Total.Value - commission;

                    // create transaction
                    var newTransaction = new Repository.Entities.Transaction
                    {
                        WalletId = storeWallet.Id,
                        TransactionType = TransactionType.IN.ToString(),
                        Amount = (int) transactionAmount,
                        Description = $"Thanh toán cho đơn hàng {order.Code}",
                        Status = TransactionStatus.PENDING.ToString()
                    };

                    await _unitOfWork.TransactionRepository.AddAsync(newTransaction);

                    // implement payos here
                    var payment = await _paymentService.CreatePaymentLinkOrder(order.Total.Value, order);

                    _unitOfWork.Save();

                    return payment;
                }
            }
            throw new Exception("Đơn hàng không tồn tại hoặc không phải đơn hàng của bạn");
        }

        public async Task<bool> CancelOrderAsync(ConfirmOrderModel orderModel, string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderModel.OrderId);
            if (order != null && order.UserId == currentUser.Id)
            {
                if (order.Status == OrderStatus.PENDING.ToString()
                    && order.PaymentStatus == PaymentStatus.PENDING.ToString())
                {
                    // delete order
                    _unitOfWork.OrderRepository.PermanentDeletedAsync(order);
                    _unitOfWork.Save();

                    return true;
                }
            }
            throw new Exception("Đơn hàng không tồn tại hoặc không phải đơn hàng của bạn");
        }

        private static string GenerateOrderCode(int storeId)
        {
            string formattedStoreId = storeId.ToString("D2");
            string timestamp = CommonUtils.GetCurrentTime().ToString("yyyyMMddHHmmss");
            return $"ST{formattedStoreId}_OD_{timestamp}";
        }
    }
}
