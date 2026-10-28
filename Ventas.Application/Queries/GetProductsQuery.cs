using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Application.ValueObjects;
using Ventas.Persistence.Model;

namespace Ventas.Application.Queries
{
    public class GetProductsQuery : IGetProductsQuery
    {
        private readonly VentasContext _productContext;

        public GetProductsQuery(VentasContext productContext)
        {
            _productContext = productContext;
        }

        public List<Producto> Execute()
        {
            var productos = _productContext.Products.Select( dbProd => new Producto
            {
                ProductId = dbProd.ProductId,               
                ProductName = dbProd.Name,
                CategoryId = dbProd.CategoryId,
                Price = dbProd.Price,
                Image  = dbProd.Image,
                CategoryName = dbProd.Category.Name

            }).ToList();

            return productos;

        }

    }
}
