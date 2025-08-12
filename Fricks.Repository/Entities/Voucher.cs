using Fricks.Repository.Enum;
using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class Voucher : BaseEntity
{
    public int? StoreId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? DiscountPercent { get; set; }

    public int? MaxDiscount { get; set; }

    public int? MinOrderValue { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? ExpireDate { get; set; }

    public string? Status { get; set; }

    public string? Availability { get; set; } = AvailabilityVoucher.STORE.ToString();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Store? Store { get; set; }
}
