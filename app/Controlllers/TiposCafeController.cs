using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApi.DTOs;
using MiApi.Entities;
using MiApi.Services;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TiposCafeController : ControllerBase
{
    private readonly ITipoCafeService _tipoCafeService;

    public TiposCafeController(ITipoCafeService tipoCafeService)
    {
        _tipoCafeService = tipoCafeService;
    }

    // GET: api/tiposcafe
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoCafeResponseDto>>> GetAll()
    {
        var tipos = await _tipoCafeService.GetAllAsync();
        return Ok(tipos.Select(MapToDto));
    }

    // GET: api/tiposcafe/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TipoCafeResponseDto>> GetById(int id)
    {
        var tipo = await _tipoCafeService.GetByIdAsync(id);
        if (tipo == null)
            return NotFound(new { message = $"No se encontró el tipo de café con ID {id}." });

        return Ok(MapToDto(tipo));
    }

    // POST: api/tiposcafe
    [HttpPost]
    public async Task<ActionResult<TipoCafeResponseDto>> Create([FromBody] TipoCafeCreateDto dto)
    {
        var tipoCafe = new TipoCafe
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        try
        {
            var creado = await _tipoCafeService.CreateAsync(tipoCafe);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, MapToDto(creado));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/tiposcafe/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TipoCafeUpdateDto dto)
    {
        var tipoCafe = new TipoCafe
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        try
        {
            await _tipoCafeService.UpdateAsync(id, tipoCafe);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/tiposcafe/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _tipoCafeService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private static TipoCafeResponseDto MapToDto(TipoCafe tipoCafe)
    {
        return new TipoCafeResponseDto
        {
            Id = tipoCafe.Id,
            Nombre = tipoCafe.Nombre,
            Descripcion = tipoCafe.Descripcion
        };
    }
}