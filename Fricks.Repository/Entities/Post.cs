using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class Post : BaseEntity
{
    public int? ProductId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Image { get; set; }

    public virtual Product? Product { get; set; }
}
