using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.CategoryModels
{
    public class CategoryProcessModel
    {
        [Required(ErrorMessage = "Danh mục sản phẩm không được để trống")]
        public required string Name { get; set; }
    }
}
