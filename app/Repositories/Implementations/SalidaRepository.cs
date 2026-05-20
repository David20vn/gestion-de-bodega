using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Entities;

namespace MiApi.Repositories;

public class SalidaRepository : ISalidaRepository
{
    private readonly AppDbContext _context;

    public SalidaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovimientoSalida>> GetAllAsync(
        int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta)
    {
        var query = _context.MovimientosSalida
            .Include(s => s.Cliente)
            .Include(s => s.Seccion)
            .Include(s => s.TipoCafe)
            .Include(s => s.AdminUser)
            .AsQueryable();

        if (clienteId.HasValue)
            query = query.Where(s => s.ClienteId == clienteId.Value);
        if (seccionId.HasValue)
            query = query.Where(s => s.SeccionId == seccionId.Value);
        if (tipoCafeId.HasValue)
            query = query.Where(s => s.TipoCafeId == tipoCafeId.Value);
        if (desde.HasValue)
            query = query.Where(s => s.Fecha >= desde.Value);
        if (hasta.HasValue)
            query = query.Where(s => s.Fecha <= hasta.Value);

        return await query.OrderByDescending(s => s.Fecha).ToListAsync();
    }

    public async Task<MovimientoSalida?> GetByIdAsync(int id)
    {
        return await _context.MovimientosSalida
            .Include(s => s.Cliente)
            .Include(s => s.Seccion)
            .Include(s => s.TipoCafe)
            .Include(s => s.AdminUser)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<MovimientoSalida> AddAsync(MovimientoSalida salida)
    {
        _context.MovimientosSalida.Add(salida);
        await _context.SaveChangesAsync();
        return salida;
    }
}