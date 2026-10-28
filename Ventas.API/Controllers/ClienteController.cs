using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Command;
using Ventas.Application.Queries;
using Ventas.Application.ValueObjects;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IGetClientsQuery _getClientsQuery;
        private readonly ICreateClientCommand _createClientCommand;
        private readonly IUpdateClientCommand _updateClientCommand;
        private readonly IDeleteClientCommand _deleteClientCommand;

        public ClienteController(
            IGetClientsQuery getClientsQuery,
            ICreateClientCommand createClientCommand,
            IUpdateClientCommand updateClientCommand,
            IDeleteClientCommand deleteClientCommand)
        {
            _getClientsQuery = getClientsQuery;
            _createClientCommand = createClientCommand;
            _updateClientCommand = updateClientCommand;
            _deleteClientCommand = deleteClientCommand;
        }

        [HttpGet(Name = "clients")]
        public List<Cliente> ObtenerClientes()
        {
            return _getClientsQuery.Execute();
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerClientePorId(int id)
        {
            var cliente = _getClientsQuery.GetById(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpGet("search")]
        public IActionResult ObtenerClientePorNombre([FromQuery] string name)
        {
            var cliente = _getClientsQuery.GetByName(name);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost]
        public IActionResult CrearCliente([FromBody] Cliente cliente)
        {
            try
            {
                var creado = _createClientCommand.Execute(cliente);
                return Ok(creado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateCliente([FromBody] Cliente cliente)
        {
            try
            {
                var actualizado = _updateClientCommand.Execute(cliente);
                return Ok(actualizado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteCliente([FromRoute] int id)
        {
            try
            {
                var borrado = _deleteClientCommand.Execute(id);
                return Ok(borrado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
