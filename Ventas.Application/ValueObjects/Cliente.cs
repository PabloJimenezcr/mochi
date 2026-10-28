using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ventas.Application.ValueObjects
{
    public class Cliente
    {
        public int ClientId { get; set; }

        public string Identifier { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
