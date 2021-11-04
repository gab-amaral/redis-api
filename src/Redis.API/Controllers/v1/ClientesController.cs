using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Redis.API.Models.v1;
using Redis.Domain.Interfaces.Services;
using Redis.Domain.Models;

namespace Redis.API.Controllers.v1
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v1/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [ProducesResponseType(typeof(Cliente), statusCode: (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Guid), statusCode: (int)HttpStatusCode.NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var cliente = await _clienteService.GetClienteAsync(id);

            if (cliente == null)
                return NotFound(id);

            return Ok(cliente);
        }

        [ProducesResponseType(typeof(IEnumerable<string>), statusCode: (int)HttpStatusCode.OK)]
        [HttpGet]
        public IActionResult GetAllAsync()
        {
            var keys = _clienteService.GetClientes();

            return Ok(keys);
        }

        [ProducesResponseType(typeof(Cliente), statusCode: (int)HttpStatusCode.OK)]
        [HttpPost]
        public async Task<IActionResult> SetAsync([FromBody] ClienteModel model)
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                PrimeiroNome = model.PrimeiroNome,
                UltimoNome = model.UltimoNome
            };

            await _clienteService.SetClienteAsync(cliente);

            return Ok(cliente);
        }

        [ProducesResponseType(typeof(bool), statusCode: (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Guid), statusCode: (int)HttpStatusCode.NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SetAsync(Guid id)
        {
            var cliente = await _clienteService.GetClienteAsync(id);

            if (cliente == null)
                return NotFound(id);

            return Ok(await _clienteService.DeleteClienteAsync(id));
        }
    }
}
