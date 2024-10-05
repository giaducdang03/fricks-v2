using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class Store : BaseEntity
{
    public int? ManagerId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? TaxCode { get; set; }

    public string? Image { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Description { get; set; }

    public string? BankCode { get; set; }

    public string? AccountNumber { get; set; }

    public virtual User? Manager { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();

    public virtual Wallet? Wallet { get; set; }
}
