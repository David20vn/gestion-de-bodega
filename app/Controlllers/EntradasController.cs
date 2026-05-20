using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApi.DTOs;
using MiApi.Entities;
using MiApi.Services;
using System.Security.Claims;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntradasController : ControllerBase
{
    private readonly IEntradaService _entradaService;

    public EntradasController(IEntradaService entradaService)
    {
        _entradaService = entradaService;
    }

    // GET: api/entradas
    // Parámetros opcionales: clienteId, seccionId, tipoCafeId, desde, hasta
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EntradaResponseDto>>> GetAll(
        [FromQuery] int? clienteId,
        [FromQuery] int? seccionId,
        [FromQuery] int? tipoCafeId,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta)
    {
        var entradas = await _entradaService.GetAllAsync(clienteId, seccionId, tipoCafeId, desde, hasta);
        return Ok(entradas.Select(MapToDto));
    }

    // GET: api/entradas/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EntradaResponseDto>> GetById(int id)
    {
        var entrada = await _entradaService.GetByIdAsync(id);
        if (entrada == null)
            return NotFound(new { message = $"No se encontró el movimiento de entrada con ID {id}." });

        return Ok(MapToDto(entrada));
    }

    // POST: api/entradas
    [HttpPost]
    public async Task<ActionResult<EntradaResponseDto>> Create([FromBody] EntradaCreateDto dto)
    {
        // Obtener el ID del administrador desde el token JWT
        var adminUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (adminUserIdClaim == null || !int.TryParse(adminUserIdClaim, out int adminUserId))
            return Unauthorized(new { message = "Token inválido o falta el identificador de usuario." });

        try
        {
            var entrada = await _entradaService.RegistrarEntradaAsync(
                adminUserId,
                dto.ClienteId,
                dto.SeccionId,
                dto.TipoCafeId,
                dto.Cantidad,
                dto.Notas
            );

            return CreatedAtAction(nameof(GetById), new { id = entrada.Id }, MapToDto(entrada));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static EntradaResponseDto MapToDto(MovimientoEntrada entrada)
    {
        return new EntradaResponseDto
        {
            Id = entrada.Id,
            ClienteId = entrada.ClienteId,
            ClienteNombre = entrada.Cliente?.Nombre ?? "",
            SeccionId = entrada.SeccionId,
            SeccionNombre = entrada.Seccion?.Nombre ?? "",
            TipoCafeId = entrada.TipoCafeId,
            TipoCafeNombre = entrada.TipoCafe?.Nombre ?? "",
            Cantidad = entrada.Cantidad,
            Fecha = entrada.Fecha,
            AdminUserId = entrada.AdminUserId,
            AdminUsername = entrada.AdminUser?.Username ?? "",
            Notas = entrada.Notas
        };
    }
}