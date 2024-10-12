using Fricks.Repository.Entities;
using Fricks.Repository.Repositories;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FricksContext _context;
        private IDbContextTransaction _transaction;
        private IBrandRepository _brandRepository;
        private IUserRepository _userRepository;
        private IOtpRepository _otpRepository;
        private IFavoriteProductRepository _favoriteProductRepository;
        private IStoreRepository _storeRepository;
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        private IProductUnitRepository _productUnitRepository;
        private IProductPriceRepository _productPriceRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IOrderRepository _orderRepository;
        private IPostRepository _postRepository;
        private IFeedbackRepository _feedbackRepository;
        private IWalletRepository _walletRepository;
        private ITransactionRepository _transactionRepository;
        private IWithdrawRepository _withdrawRepository;

        public UnitOfWork(FricksContext context) 
        { 
            _context = context;
            _transaction = _context.Database.BeginTransaction();
        }
        public IUserRepository UsersRepository
        {
            get
            {
                return _userRepository ??= new UserRepository(_context);

            }
        }

        public IOtpRepository OtpsRepository
        {
            get
            {
                return _otpRepository ??= new OtpRepository(_context);
            }
        }

        public IBrandRepository BrandRepository 
        {
            get
            {
                return _brandRepository ??= new BrandRepository(_context);
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository ??= new CategoryRepository(_context);
            }
        }

        public IFavoriteProductRepository FavoriteProductRepository
        {
            get
            {
                return _favoriteProductRepository ??= new FavoriteProductRepository(_context);
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                return _productRepository ??= new ProductRepository(_context);
            }
        }

        public IStoreRepository StoreRepository 
        {
            get
            {
                return _storeRepository ??= new StoreRepository(_context);
            }
        }

        public IProductUnitRepository ProductUnitRepository
        {
            get
            {
                return _productUnitRepository ??= new ProductUnitRepository(_context);
            }
        }

        public IProductPriceRepository ProductPriceRepository
        {
            get
            {
                return _productPriceRepository ??= new ProductPriceRepository(_context);
            }
        }

        public IPostRepository PostRepository
        {
            get
            {
                return _postRepository ??= new PostRepository(_context);
            }
        }

        public IFeedbackRepository FeedbackRepository
        {
            get
            {
                return _feedbackRepository ??= new FeedbackRepository(_context);
            }
        }

        public IWalletRepository WalletRepository
        {
            get
            {
                return _walletRepository ??= new WalletRepository(_context);
            }
        }

        public ITransactionRepository TransactionRepository
        {
            get
            {
                return _transactionRepository ??= new TransactionRepository(_context);
            }
        }

        public IWithdrawRepository WithdrawRepository
        {
            get
            {
                return _withdrawRepository ??= new WithdrawRepository(_context);
            }
        }

        public IOrderDetailRepository OrderDetailRepository
        {
            get
            {
                return _orderDetailRepository ??= new OrderDetailRepository(_context);
            }
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                return _orderRepository ??= new OrderRepository(_context);
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction?.Commit();
            }
            catch (Exception)
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
            }
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
