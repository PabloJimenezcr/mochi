using System;
using System.Collections.Generic;

namespace Ventas.Persistence.Model;

public partial class Client
{
    public int ClientId { get; set; }

    public string Identifier { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
