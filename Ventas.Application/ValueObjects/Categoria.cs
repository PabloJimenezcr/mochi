using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ventas.Application.ValueObjects
{
    public class Categoria
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;           



    }
}
