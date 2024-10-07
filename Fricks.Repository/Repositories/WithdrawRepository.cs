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
    public class WithdrawRepository : GenericRepository<Withdraw>, IWithdrawRepository
    {
        private readonly FricksContext _context;

        public WithdrawRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

    }
}
