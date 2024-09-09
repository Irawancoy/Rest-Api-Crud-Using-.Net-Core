using System;
using System.Collections.Generic;

namespace SimpleRestaurantApp.Models;

public partial class Food
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int Stock { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
