using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.BannerModels
{
    public class BannerUpdateModel : BannerProcessModel
    {
        [Required]
        public required int Id { get; set; }
    }
}
