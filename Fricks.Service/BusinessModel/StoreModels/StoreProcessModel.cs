using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.StoreModels
{
    public class StoreProcessModel
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? TaxCode { get; set; }

        public string? Image { get; set; }
    }
}
