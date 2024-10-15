using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class OrderDetail : BaseEntity
{
    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Price { get; set; }

    public int? Quantity { get; set; }

    public string? ProductUnit { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
