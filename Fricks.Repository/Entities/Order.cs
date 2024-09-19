using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class Order : BaseEntity
{
    public string? Code { get; set; }

    public int? ShipFee { get; set; }

    public int? Discount { get; set; }

    public int? Total { get; set; }

    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? PaymentMethod { get; set; }

    public int? UserId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerAddress { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerPhone { get; set; }

    public int? StoreId { get; set; }

    public int? VoucherId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Store? Store { get; set; }

    public virtual User? User { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
