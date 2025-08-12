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
using System.Net.WebSockets;
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

            // calculate discount
            var discount = 0;

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

                // add fee ship

                var buyStore = await _unitOfWork.StoreRepository.GetStoreByIdAsync(storeId);
                if (buyStore == null)
                {
                    throw new Exception($"Cửa hàng không tồn tại");
                }

                int shipFree = buyStore.DefaultShip != null ? buyStore.DefaultShip.Value : 0;

                totalPrice += shipFree;

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

                // appled voucher

                // Tìm cả hai loại voucher
                var globalVoucher = await _unitOfWork.VoucherRepository.GetGlobalVoucherByCode(createOrderModel.VoucherCode);
                var storeVoucher = await _unitOfWork.VoucherRepository.GetVoucherByCode(createOrderModel.VoucherCode, storeId);

                Voucher? appliedVoucher = null;

                // Kiểm tra điều kiện hợp lệ cho từng voucher
                bool isGlobalValid = globalVoucher != null
                    && globalVoucher.MinOrderValue <= totalPrice
                    && (globalVoucher.Availability == null || globalVoucher.Availability == AvailabilityVoucher.GLOBAL.ToString());

                bool isStoreValid = storeVoucher != null
                    && storeVoucher.MinOrderValue <= totalPrice
                    && (storeVoucher.Availability == null || storeVoucher.Availability == AvailabilityVoucher.STORE.ToString());

                // Chọn voucher phù hợp (ưu tiên store voucher nếu cả hai đều hợp lệ)
                if (isStoreValid)
                {
                    appliedVoucher = storeVoucher;
                }
                else if (isGlobalValid)
                {
                    appliedVoucher = globalVoucher;
                }

                // Áp dụng discount nếu có voucher hợp lệ
                if (appliedVoucher != null)
                {
                    // voucher free ship (FS)
                    if (appliedVoucher.Code.StartsWith("FS") && createOrderModel.ShipFee.HasValue)
                    {
                        discount = createOrderModel.ShipFee.Value * appliedVoucher.DiscountPercent.Value / 100;
                    }
                    else
                    {
                        discount = totalPrice * appliedVoucher.DiscountPercent.Value / 100;
                    }
                }

                var newOrder = new CalculateOrderModel
                {
                    UserId = currentUser.Id,
                    CustomerEmail = currentUser.Email,
                    CustomerName = currentUser.FullName,
                    CustomerAddress = currentUser.Address,
                    StoreId = storeId,
                    Total = totalPrice - discount,
                    ShipFee = shipFree,
                    Discount = discount,
                    VoucherId = appliedVoucher != null ? appliedVoucher.Id : null,
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

            // calculate discount
            var discount = 0;
            var voucher = await _unitOfWork.VoucherRepository.GetVoucherByCode(orderModel.VoucherCode, 5); // hard code
            if (voucher != null)
            {
                if (voucher.Code.StartsWith("FS"))
                {
                    discount = orderModel.ShipFee.Value * voucher.DiscountPercent.Value / 100;
                }
            }

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

                // add fee ship

                var buyStore = await _unitOfWork.StoreRepository.GetStoreByIdAsync(storeId);
                if (buyStore == null)
                {
                    throw new Exception($"Cửa hàng không tồn tại");
                }

                int shipFree = buyStore.DefaultShip != null ? buyStore.DefaultShip.Value : 0;

                totalPrice += shipFree;

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

                // add payment code
                long paymentCode = NumberUtils.GetRandomLong();

                var newOrder = new Order
                {
                    UserId = orderUser.Id,
                    CustomerEmail = orderUser.Email,
                    CustomerName = orderUser.FullName,
                    CustomerAddress = orderModel.CustomerAddress,
                    CustomerPhone = orderModel.CustomerPhone,
                    Status = OrderStatus.PENDING.ToString(),
                    PaymentMethod = orderModel.PaymentMethod.ToString(),
                    PaymentStatus = PaymentStatus.PENDING.ToString(),
                    Code = GenerateOrderCode(storeId, paymentCode),
                    StoreId = storeId,
                    Total = totalPrice - discount,
                    ShipFee = shipFree,
                    PaymentCode = paymentCode,
                    Discount = discount,
                    VoucherId = voucher != null ? voucher.Id : null,
                    OrderDetails = orderdetails
                };

                return newOrder;
            }
            throw new Exception("Không có sản phẩm nào trong đơn hàng");
        }

        public async Task<OrderModel> UpdateOrderStatus(UpdateOrderModel orderModel)
        {
            var updateOrder = await _unitOfWork.OrderRepository.GetOrderById(orderModel.Id);
            if (updateOrder != null)
            {
                if (orderModel.Status == OrderStatus.DONE)
                {
                    if (updateOrder.Status == OrderStatus.DELIVERY.ToString() && updateOrder.PaymentStatus == PaymentStatus.PAID.ToString())
                    {
                        updateOrder.Status = orderModel.Status.ToString();
                        updateOrder.DeliveryDate = CommonUtils.GetCurrentTime();
                        if (orderModel.Image != null)
                        {
                            updateOrder.DeliveryImage = orderModel.Image;
                        }

                        _unitOfWork.OrderRepository.UpdateAsync(updateOrder);
                        _unitOfWork.Save();

                        return _mapper.Map<OrderModel>(updateOrder);
                    }
                    throw new Exception("Trạng thái đơn hàng không hợp lệ");
                }
                else if (orderModel.Status == OrderStatus.CANCELED)
                {
                    if (updateOrder.Status == OrderStatus.PENDING.ToString()
                        && updateOrder.PaymentStatus == PaymentStatus.PENDING.ToString())
                    {
                        if (updateOrder.CreateDate.AddMinutes(15) <= CommonUtils.GetCurrentTime())
                        {
                            updateOrder.Status = orderModel.Status.ToString();

                            _unitOfWork.OrderRepository.UpdateAsync(updateOrder);
                            _unitOfWork.Save();

                            return _mapper.Map<OrderModel>(updateOrder);
                        }
                        else
                        {
                            throw new Exception("Bạn chỉ có thể chuyển đơn hàng sang trạng thái hủy khi chưa được thanh toán trong vòng 15 phút");
                        }
                    }
                    throw new Exception("Không thể hủy đơn hàng này");

                }
                else
                {
                    throw new Exception($"Không thể cập nhật trạng thái {orderModel.Status.ToString()} cho đơn hàng này");
                }
            }
            throw new Exception("Đơn hàng không hợp lệ");
        }

        private static string GenerateOrderCode(int storeId, long paymentCode)
        {
            string formattedStoreId = storeId.ToString("D3");
            //string timestamp = CommonUtils.GetCurrentTime().ToString("yyyyMMddHHmmss");
            return $"ST{formattedStoreId}_OD_{paymentCode}";
        }

        public async Task<List<OrderModel>> GetAllOrdersAsync(string email)
        {
            var currentUser = await _unitOfWork.UsersRepository.GetUserByEmail(email);
            if (currentUser == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            if (currentUser.Role == RoleEnums.ADMIN.ToString())
            {
                return _mapper.Map<List<OrderModel>>(await _unitOfWork.OrderRepository.GetAllOrderAsync());
            }
            else
            {
                var currentStore = await _unitOfWork.StoreRepository.GetStoreByManagerId(currentUser.Id);
                if (currentStore == null)
                {
                    throw new Exception("Cửa hàng không tồn tại");
                }

                var allOrders = await _unitOfWork.OrderRepository.GetAllOrderAsync();
                return _mapper.Map<List<OrderModel>>(allOrders.Where(x => x.StoreId == currentStore.Id));
            }
        }
    }
}
