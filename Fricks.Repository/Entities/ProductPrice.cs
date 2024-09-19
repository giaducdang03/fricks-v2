using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class ProductPrice : BaseEntity
{
    public int? ProductId { get; set; }

    public int? UnitId { get; set; }

    public int? Price { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ProductUnit? Unit { get; set; }
}
