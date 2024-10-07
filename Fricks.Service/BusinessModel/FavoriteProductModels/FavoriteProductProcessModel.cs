using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.ProductModels;
using Fricks.Service.BusinessModel.UserModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.FavoriteProductModels
{
    public class FavoriteProductProcessModel
    {
        [Required]
        public int ProductId { get; set; }
    }
}
