using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class Product : BaseEntity
{
    public string? Sku { get; set; }

    public string? UnsignName { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public int? StoreId { get; set; }

    public int? SoldQuantity { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<FavoriteProduct> FavoriteProducts { get; set; } = new List<FavoriteProduct>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual Store? Store { get; set; }
}
