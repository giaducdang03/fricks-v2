using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.PostModels
{
    public class PostModel : BaseEntity
    {
        public int? ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Image { get; set; }

        //public virtual ProductModel? Product { get; set; }
    }
}
