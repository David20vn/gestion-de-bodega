using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApi.DTOs;
using MiApi.Services;
using MiApi.Entities;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventarioController : ControllerBase
{
    private readonly IInventarioService _inventarioService;

    public InventarioController(IInventarioService inventarioService)
    {
        _inventarioService = inventarioService;
    }

    // GET: api/inventario
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventarioResponseDto>>> GetAll()
    {
        var inventarios = await _inventarioService.GetAllAsync();
        return Ok(inventarios.Select(MapToDto));
    }

    // GET: api/inventario/seccion/1
    [HttpGet("seccion/{seccionId:int}")]
    public async Task<ActionResult<IEnumerable<InventarioResponseDto>>> GetBySeccion(int seccionId)
    {
        var inventarios = await _inventarioService.GetBySeccionAsync(seccionId);
        return Ok(inventarios.Select(MapToDto));
    }

    // GET: api/inventario/seccion/1/tipocafe/2
    [HttpGet("seccion/{seccionId:int}/tipocafe/{tipoCafeId:int}")]
    public async Task<ActionResult<InventarioResponseDto>> GetBySeccionAndTipo(int seccionId, int tipoCafeId)
    {
        var inventario = await _inventarioService.GetBySeccionAndTipoAsync(seccionId, tipoCafeId);
        if (inventario == null)
            return NotFound(new { message = "No hay inventario registrado para esta combinación." });

        return Ok(MapToDto(inventario));
    }

    // POST: api/inventario (registrar o actualizar manualmente)
    [HttpPost]
    public async Task<ActionResult<InventarioResponseDto>> RegistrarOActualizar([FromBody] RegistrarInventarioRequestDto dto)
    {
        try
        {
            var inventario = await _inventarioService.RegistrarOActualizarAsync(dto.SeccionId, dto.TipoCafeId, dto.Cantidad);
            return Ok(MapToDto(inventario)); // 200 porque puede ser creación o actualización
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static InventarioResponseDto MapToDto(Inventario inventario)
    {
        return new InventarioResponseDto
        {
            Id = inventario.Id,
            SeccionId = inventario.SeccionId,
            SeccionNombre = inventario.Seccion?.Nombre ?? "",
            TipoCafeId = inventario.TipoCafeId,
            TipoCafeNombre = inventario.TipoCafe?.Nombre ?? "",
            Cantidad = inventario.Cantidad
        };
    }
}