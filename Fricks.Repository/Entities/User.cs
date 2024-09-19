using System;
using System.Collections.Generic;

namespace Fricks.Repository.Entities;

public partial class User : BaseEntity
{
    public string? Email { get; set; }

    public bool? ConfirmEmail { get; set; }

    public string? PasswordHash { get; set; }

    public string? GoogleId { get; set; }

    public string? Avatar { get; set; }

    public string? DeviceToken { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<FavoriteProduct> FavoriteProducts { get; set; } = new List<FavoriteProduct>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
