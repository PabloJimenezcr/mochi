using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;

namespace Ventas.Application.Queries
{
    public class GetCategoriesQuery : IGetCategoriesQuery
    {
        private readonly VentasContext _categoriesContext;

        public GetCategoriesQuery(VentasContext categoriesContext)
        {
            _categoriesContext = categoriesContext;
        }

        public List<Categoria> Execute()
        {    
            var categorias = _categoriesContext.Categories.Select(dbCat => new Categoria
            {
                CategoryId = dbCat.CategoryId,
                CategoryName = dbCat.Name

            }).ToList();

            return categorias;

        }

        internal bool EstaActivo()
        {
            return true;
        }

    }
}
