using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.ValueObjects;
using Ventas.Application.Queries;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IGetProductsQuery _getProductsQuery;

        public ProductoController(IGetProductsQuery getProductsQuery)
        {
            _getProductsQuery = getProductsQuery;
        }

        [HttpGet(Name = "products")]
        public List<Producto> ObtenerProductos()
        {
            return _getProductsQuery.Execute();
        }

    }
}
