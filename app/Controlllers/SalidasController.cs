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
public class SalidasController : ControllerBase
{
    private readonly ISalidaService _salidaService;

    public SalidasController(ISalidaService salidaService)
    {
        _salidaService = salidaService;
    }

    // GET: api/salidas
    // Parámetros opcionales: clienteId, seccionId, tipoCafeId, desde, hasta
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalidaResponseDto>>> GetAll(
        [FromQuery] int? clienteId,
        [FromQuery] int? seccionId,
        [FromQuery] int? tipoCafeId,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta)
    {
        var salidas = await _salidaService.GetAllAsync(clienteId, seccionId, tipoCafeId, desde, hasta);
        return Ok(salidas.Select(MapToDto));
    }

    // GET: api/salidas/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SalidaResponseDto>> GetById(int id)
    {
        var salida = await _salidaService.GetByIdAsync(id);
        if (salida == null)
            return NotFound(new { message = $"No se encontró el movimiento de salida con ID {id}." });

        return Ok(MapToDto(salida));
    }

    // POST: api/salidas
    [HttpPost]
    public async Task<ActionResult<SalidaResponseDto>> Create([FromBody] SalidaCreateDto dto)
    {
        // Obtener el ID del administrador desde el token JWT
        var adminUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (adminUserIdClaim == null || !int.TryParse(adminUserIdClaim, out int adminUserId))
            return Unauthorized(new { message = "Token inválido o falta el identificador de usuario." });

        try
        {
            var salida = await _salidaService.RegistrarSalidaAsync(
                adminUserId,
                dto.ClienteId,
                dto.SeccionId,
                dto.TipoCafeId,
                dto.Cantidad,
                dto.Notas
            );

            return CreatedAtAction(nameof(GetById), new { id = salida.Id }, MapToDto(salida));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static SalidaResponseDto MapToDto(MovimientoSalida salida)
    {
        return new SalidaResponseDto
        {
            Id = salida.Id,
            ClienteId = salida.ClienteId,
            ClienteNombre = salida.Cliente?.Nombre ?? "",
            SeccionId = salida.SeccionId,
            SeccionNombre = salida.Seccion?.Nombre ?? "",
            TipoCafeId = salida.TipoCafeId,
            TipoCafeNombre = salida.TipoCafe?.Nombre ?? "",
            Cantidad = salida.Cantidad,
            Fecha = salida.Fecha,
            AdminUserId = salida.AdminUserId,
            AdminUsername = salida.AdminUser?.Username ?? "",
            Notas = salida.Notas
        };
    }
}