using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Application.ValueObjects;

namespace Ventas.Application.Queries
{
    public class GetCategoriesFromAPIQuery : IGetCategoriesQuery
    {
        public List<Categoria> Execute()
        {
            return new List<Categoria>
            {
                new Categoria () { CategoryId = 10, CategoryName = "Electrodomesticos API"},
                new Categoria () { CategoryId = 11, CategoryName = "Deportes API"},
                new Categoria () { CategoryId = 12, CategoryName = "Hogar API"}
            };
        }
    }
}
