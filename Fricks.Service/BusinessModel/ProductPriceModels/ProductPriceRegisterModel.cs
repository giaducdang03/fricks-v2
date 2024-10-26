using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.ProductUnitModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.ProductPriceModels
{
    public class ProductPriceRegisterModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UnitCode { get; set; } = "";

        [Required]
        public int Price { get; set; }
    }
}
