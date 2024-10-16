using Fricks.Repository.Entities;
using Fricks.Repository.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository.Repositories
{
    public class BannerRepository : GenericRepository<Banner>, IBannerRepository
    {
        public BannerRepository(FricksContext context) : base(context)
        {
        }
    }
}
