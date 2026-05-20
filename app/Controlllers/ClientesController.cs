using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApi.DTOs;
using MiApi.Entities;
using MiApi.Services;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos los endpoints requieren autenticación JWT
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    // GET: api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetAll()
    {
        var clientes = await _clienteService.GetAllAsync();
        var response = clientes.Select(c => MapToDto(c));
        return Ok(response);
    }

    // GET: api/clientes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponseDto>> GetById(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente == null)
            return NotFound(new { message = $"No se encontró el cliente con ID {id}." });

        return Ok(MapToDto(cliente));
    }

    // POST: api/clientes
    [HttpPost]
    public async Task<ActionResult<ClienteResponseDto>> Create([FromBody] ClienteCreateDto dto)
    {
        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Contacto = dto.Contacto,
            Telefono = dto.Telefono,
            Email = dto.Email
        };

        var creado = await _clienteService.CreateAsync(cliente);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, MapToDto(creado));
    }

    // PUT: api/clientes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDto dto)
    {
        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Contacto = dto.Contacto,
            Telefono = dto.Telefono,
            Email = dto.Email
        };

        await _clienteService.UpdateAsync(id, cliente);
        return NoContent();
    }

    // DELETE: api/clientes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _clienteService.DeleteAsync(id);
        return NoContent();
    }

    // Mapeo simple interno
    private static ClienteResponseDto MapToDto(Cliente cliente)
    {
        return new ClienteResponseDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Contacto = cliente.Contacto,
            Telefono = cliente.Telefono,
            Email = cliente.Email,
            FechaRegistro = cliente.FechaRegistro
        };
    }
}