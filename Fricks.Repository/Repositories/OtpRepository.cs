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
    public class OtpRepository : GenericRepository<Otp>, IOtpRepository
    {
        private readonly FricksContext _context;

        public OtpRepository(FricksContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Otp> GetOtpByCode(string otpCode)
        {
            return await _context.Otps.FirstOrDefaultAsync(x => x.OtpCode == otpCode);
        }
    }
}
