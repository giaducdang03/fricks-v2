using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.OrderModels;
using Fricks.Service.BusinessModel.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.OrderDetailModels
{
    public class OrderDetailModel
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? ProductId { get; set; }

        public int? Price { get; set; }

        public int? Quantity { get; set; }

        public string? ProductUnit { get; set; }

        public ProductModel? Product { get; set; }
    }
}
