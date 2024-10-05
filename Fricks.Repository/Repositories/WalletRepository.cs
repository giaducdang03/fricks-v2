using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories
{
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly FricksContext _context;

        public WalletRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Wallet> GetWalletStoreAsync(int storeId)
        {
            return await _context.Wallets.Include(x => x.Store).FirstOrDefaultAsync(x => x.StoreId == storeId);
        }
    }
}
