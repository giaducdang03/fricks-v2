using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.BusinessModel.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.FavoriteProductModels
{
    public class FavoriteProductProcessModel
    {
        public int? UserId { get; set; }

        public int? ProductId { get; set; }
    }
}
