using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class Category : BaseEntity
{
    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
