using AutoMapper;
using Fricks.Repository.Commons;
using Fricks.Repository.Commons.Filters;
using Fricks.Repository.Entities;
using Fricks.Repository.Enum;
using Fricks.Repository.UnitOfWork;
using Fricks.Repository.Utils;
using Fricks.Service.BusinessModel.OrderDetailModels;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.BusinessModel.PaymentModels;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.Services.Interface;
using Fricks.Service.Utils;
using Microsoft.AspNetCore.Http;
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
            return await _paymentService.CreatePayOsLinkOrder(orderProcessModel.Total, addOrder);
        }

        public async Task<CreatePaymentResult> RequestPaymentOrderAsync(PaymentOrderModel orderModel, HttpContext httpContext)
        {
            try
            {
                var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(orderModel.CustomerEmail);
                if (currentUser == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }

                var newOrder = await CalculatePriceOrder(orderModel, currentUser);

                if (newOrder != null)
                {
                    await _unitOfWork.OrderRepository.AddAsync(newOrder);
                    _unitOfWork.Save();

                    CreatePaymentResult payment = null;

                    if (newOrder.PaymentMethod == PaymentMethod.VIETQR.ToString())
                    {
                        payment = await _paymentService.CreatePayOsLinkOrder(newOrder.Total.Value, newOrder);
                    }
                    else if (newOrder.PaymentMethod == PaymentMethod.VNPAY.ToString())
                    {
                        payment = _paymentService.CreateVnpayLinkOrder(newOrder, httpContext);
                    }

                    return payment;
                }
                return null;
            }
            catch
            {
                throw;
            }
            

        }

        public Task<Pagination<OrderModel>> GetOrderByStoreId(int storeId)
        {
            throw new NotImplementedException();
        }

        public async Task<Pagination<OrderModel>> GetOrderPaging(string email, PaginationParameter paginationParameter, OrderFilter orderFilter)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser != null)
            {
                if (currentUser.Role == RoleEnums.CUSTOMER.ToString())
                {
                    return _mapper.Map<Pagination<OrderModel>>(await _unitOfWork.OrderRepository.GetOrderPaging(currentUser.Id, paginationParameter, orderFilter));
                }
                else if (currentUser.Role == RoleEnums.STORE.ToString())
                {
                    var store = await _unitOfWork.StoreRepository.GetStoreByManagerId(currentUser.Id);
                    if (store != null)
                    {
                        orderFilter.StoreId = store.Id;
                        return _mapper.Map<Pagination<OrderModel>>(await _unitOfWork.OrderRepository.GetOrderPaging(0, paginationParameter, orderFilter));
                    }
                    throw new Exception("Cửa hàng không tồn tại");
                }
                else
                {
                    return _mapper.Map<Pagination<OrderModel>>(await _unitOfWork.OrderRepository.GetOrderPaging(0, paginationParameter, orderFilter));
                }
            }
            else
            {
                throw new Exception("Tài khoản không tồn tại");
            }
        }

        public async Task<OrderModel> GetOrderById(int id)
        {
            return _mapper.Map<OrderModel>(await _unitOfWork.OrderRepository.GetOrderById(id));
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
                    && order.PaymentStatus == PaymentStatus.PENDING.ToString())
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
                        Amount = (int)transactionAmount,
                        Description = $"Thanh toán cho đơn hàng {order.Code}",
                        Status = TransactionStatus.PENDING.ToString()
                    };

                    await _unitOfWork.TransactionRepository.AddAsync(newTransaction);

                    // implement payos here
                    var payment = await _paymentService.CreatePayOsLinkOrder(order.Total.Value, order);

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

        public async Task<CalculateOrderModel> CalculateOrderAsync(CreateOrderModel createOrderModel)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(createOrderModel.CustomerEmail);
            if (currentUser == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            var dbProduct = await _unitOfWork.ProductRepository.GetAllProductsAsync();
            var validProductUnits = await _unitOfWork.ProductUnitRepository.GetAllAsync();

            if (createOrderModel.ProductOrders.Count > 0)
            {
                // check list product
                bool isProductValid = true;
                foreach (var item in createOrderModel.ProductOrders)
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

                var orderedProducts = dbProduct.Where(p => createOrderModel.ProductOrders.Any(o => o.ProductId == p.Id));

                // check store
                bool isOneStore = orderedProducts.All(p => p.StoreId == orderedProducts.First().StoreId);
                if (!isOneStore)
                {
                    throw new Exception("Mỗi đơn hàng chỉ có thể mua sản phẩm của cùng một cửa hàng");
                }

                int storeId = orderedProducts.First().StoreId.Value;

                int totalPrice = 0;

                List<CalculateOrderDetailModel> orderdetails = new List<CalculateOrderDetailModel>();
                foreach (var order in orderedProducts)
                {
                    // get price unit
                    var temp = createOrderModel.ProductOrders.FirstOrDefault(x => x.ProductId == order.Id);
                    if (temp != null)
                    {
                        var temp2 = order.ProductPrices.FirstOrDefault(x => x.Unit.Id == temp.ProductUnitId);
                        if (temp2 != null)
                        {
                            totalPrice += temp2.Price.Value * temp.Quantity;
                            var newOrderDetail = new CalculateOrderDetailModel
                            {
                                ProductId = temp2.ProductId,
                                Price = temp2.Price.Value,
                                Quantity = temp.Quantity,
                                Product = new CalculateProductOrderDetailModel
                                {
                                    Sku = order.Sku,
                                    Name = order.Name,
                                    Image = order.Image,
                                    StoreName = order.Store.Name,
                                    UnitId = temp2.UnitId,
                                    UnitName = temp2.Unit.Name
                                }
                            };
                            orderdetails.Add(newOrderDetail);
                        }
                    }
                }

                var newOrder = new CalculateOrderModel
                {
                    UserId = currentUser.Id,
                    CustomerEmail = currentUser.Email,
                    CustomerName = currentUser.FullName,
                    CustomerAddress = currentUser.Address,
                    StoreId = storeId,
                    Total = totalPrice,
                    OrderDetails = orderdetails
                };

                return newOrder;

            }
            throw new Exception("Không có sản phẩm nào trong đơn hàng");
        }

        private async Task<Order> CalculatePriceOrder(PaymentOrderModel orderModel, User orderUser)
        {
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

                // check quantity stock product
                bool isInStockQuantity = true;
                foreach (var item in orderModel.ProductOrders)
                {
                    var productInStock = dbProduct.FirstOrDefault(x => x.Id == item.ProductId && x.Quantity >= item.Quantity);
                    if (productInStock == null)
                    {
                        isInStockQuantity = false;
                        break;
                    }
                }

                if (!isInStockQuantity)
                {
                    throw new Exception("Không đủ số lượng sản phẩm trong kho");
                }

                var orderedProducts = dbProduct.Where(p => orderModel.ProductOrders.Any(o => o.ProductId == p.Id));

                // check store
                bool isOneStore = orderedProducts.All(p => p.StoreId == orderedProducts.First().StoreId);
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
                                ProductUnit = temp2.Unit.Name,
                                CreateDate = CommonUtils.GetCurrentTime(),
                            };
                            orderdetails.Add(newOrderDetail);
                        }
                    }
                }

                var newOrder = new Order
                {
                    UserId = orderUser.Id,
                    CustomerEmail = orderUser.Email,
                    CustomerName = orderUser.FullName,
                    CustomerAddress = orderUser.Address != null ? orderUser.Address : orderModel.CustomerAddress,
                    CustomerPhone = orderUser.PhoneNumber != null ? orderUser.PhoneNumber : orderModel.CustomerPhone,
                    Status = OrderStatus.PENDING.ToString(),
                    PaymentMethod = orderModel.PaymentMethod.ToString(),
                    PaymentStatus = PaymentStatus.PENDING.ToString(),
                    Code = GenerateOrderCode(storeId),
                    StoreId = storeId,
                    Total = totalPrice,
                    OrderDetails = orderdetails
                };

                return newOrder;
            }
            throw new Exception("Không có sản phẩm nào trong đơn hàng");
        }

        private static string GenerateOrderCode(int storeId)
        {
            string formattedStoreId = storeId.ToString("D2");
            string timestamp = CommonUtils.GetCurrentTime().ToString("yyyyMMddHHmmss");
            return $"ST{formattedStoreId}_OD_{timestamp}";
        }
    }
}
