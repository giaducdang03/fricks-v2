using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class FavoriteProduct : BaseEntity
{
    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
