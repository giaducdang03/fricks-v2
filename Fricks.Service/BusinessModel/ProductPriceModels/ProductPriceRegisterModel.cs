using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.ProductUnitModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.ProductPriceModels
{
    public class ProductPriceRegisterModel
    {
        public int? ProductId { get; set; }

        public int? UnitId { get; set; }

        public int? Price { get; set; }
    }
}
