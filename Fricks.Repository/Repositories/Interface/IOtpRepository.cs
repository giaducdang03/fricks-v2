using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories.Interface
{
    public interface IOtpRepository : IGenericRepository<Otp>
    {
        public Task<Otp> GetOtpByCode(string otpCodde);
    }
}
