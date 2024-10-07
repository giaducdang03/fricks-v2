using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UsersRepository { get; }
        IBrandRepository BrandRepository { get; }
        IOtpRepository OtpsRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IFavoriteProductRepository FavoriteProductRepository { get; }
        IStoreRepository StoreRepository { get; }
        IProductUnitRepository ProductUnitRepository { get; }
        IProductPriceRepository ProductPriceRepository { get; }
        IPostRepository PostRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderRepository OrderRepository { get; }   
        int Save();
        void Commit();
        void Rollback();
    }
}
