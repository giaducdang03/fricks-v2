using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.BrandModels
{
    public class BrandProcessModel
    {
        [Required(ErrorMessage = "Tên hãng không được để trống")]
        public required string Name { get; set; }
    }
}
