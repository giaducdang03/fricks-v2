using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class Feedback : BaseEntity
{
    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public string? Image { get; set; }

    public string? Content { get; set; }

    public int? Rate { get; set; }

    public virtual Product? Product { get; set; }
}
