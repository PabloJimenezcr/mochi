using System;
using System.Collections.Generic;

namespace Ventas.Persistence.Model;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public byte[]? Image { get; set; }

    public virtual Category Category { get; set; } = null!;
}
