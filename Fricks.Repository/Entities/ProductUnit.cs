using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class ProductUnit : BaseEntity
{
    public string? Name { get; set; }

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
}
