using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ventas.Application.Command;
using Ventas.Application.Queries;
using Ventas.Application.ValueObjects;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ILogger<CategoriaController> _logger;
        private readonly IGetCategoriesQuery _getCategoriesQuery;
        private readonly ICreateCategoryCommand _createCategoryCommand;
        private readonly IUpdateCategoryCommand _updateCategoryCommand;
        private readonly IDeleteCategoryCommand _deleteCategoryCommand;

        public CategoriaController(
            ILogger<CategoriaController> logger, 
            IGetCategoriesQuery getCategoriesQuery,
            ICreateCategoryCommand createCategoryCommand,
            IUpdateCategoryCommand updateCategoryCommand,
            IDeleteCategoryCommand deleteCategoryCommand)
        {
            _logger = logger;
            _getCategoriesQuery = getCategoriesQuery;
            _createCategoryCommand = createCategoryCommand;
            _updateCategoryCommand = updateCategoryCommand;
            _deleteCategoryCommand = deleteCategoryCommand;
        }

        [HttpGet(Name = "categories")]
        public IActionResult ObtenerCategorias()
        {
            try
            {
                var result = _getCategoriesQuery.Execute();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al obtener categorías: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener categorías");
                return StatusCode(500, "Ocurrió un error inesperado al obtener las categorías.");
            }
        }

        [HttpPost]
        public IActionResult CrearCategoria([FromBody] Categoria categoria)
        {
            try
            {
                var categoriaCreada = _createCategoryCommand.Execute(categoria);
                return Ok(categoriaCreada);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear categoría: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear categoría");
                return StatusCode(500, "Ocurrió un error inesperado al crear la categoría.");
            }
        }

        //[HttpPost]
        //public string ActualizarCategoria([FromQuery] string name, [FromQuery] int idCategoria)
        //{
        //    try
        //    {
        //        var command = new UpdateCategoryCommand();
        //        var categoriaActualiada = command.Execute(new Categoria

        //        { CategoryId = idCategoria, CategoryName = name });


        //        return categoriaActualiada.CategoryName;
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        [HttpPut]
        public IActionResult UpdateCategoria([FromBody] Categoria categoria)
        {
            try
            {
                var categoriaCreada = _updateCategoryCommand.Execute(categoria);
                return Ok(categoriaCreada);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar categoría: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar categoría");
                return StatusCode(500, "Ocurrió un error inesperado al actualizar la categoría.");
            }

        }


        [HttpDelete]
        public IActionResult DeleteCategoria( [FromRoute]int id)
        {
            try
            {
                var borradoExitoso = _deleteCategoryCommand.Execute(id);
                return Ok(borradoExitoso);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al eliminar categoría: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar categoría");
                return StatusCode(500, "Ocurrió un error inesperado al eliminar la categoría.");
            }

        }


    }
}
