using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.CategoryModels
{
    public class CategoryModel : BaseEntity
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
