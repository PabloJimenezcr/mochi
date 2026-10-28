using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Persistence.Model;

namespace Ventas.Application.ValueObjects
{
    public class Producto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public decimal Price { get; set; }

        public byte[]? Image { get; set; }

        public string CategoryName { get; set; }

    }
}
