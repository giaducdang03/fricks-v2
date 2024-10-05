using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IWalletRepository : IGenericRepository<Wallet>
    {
        public Task<Wallet> GetWalletStoreAsync(int storeId);
    }
}
