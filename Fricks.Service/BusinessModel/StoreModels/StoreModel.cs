using Fricks.Repository.Entities;
using Fricks.Service.BusinessModel.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.StoreModels
{
    public class StoreModel : BaseEntity
    {
        public int? ManagerId { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? TaxCode { get; set; }

        public string? Image { get; set; }

        public string? PhoneNumber {  get; set; }

        public string? Description { get; set; }

        public string? ManagerEmail {  get; set; }

        public string? BankCode { get; set; }

        public string? AccountNumber { get; set; }

        public string? AccountName { get; set; }

        public int? DefaultShip { get; set; }
    }
}
