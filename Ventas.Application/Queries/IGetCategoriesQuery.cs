using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Application.ValueObjects;

namespace Ventas.Application.Queries
{
    public interface IGetCategoriesQuery
    {
        List<Categoria> Execute();
    }
}
