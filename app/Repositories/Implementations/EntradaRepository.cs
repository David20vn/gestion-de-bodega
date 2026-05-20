using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Entities;

namespace MiApi.Repositories;

public class EntradaRepository : IEntradaRepository
{
    private readonly AppDbContext _context;

    public EntradaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovimientoEntrada>> GetAllAsync(
        int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta)
    {
        var query = _context.MovimientosEntrada
            .Include(e => e.Cliente)
            .Include(e => e.Seccion)
            .Include(e => e.TipoCafe)
            .Include(e => e.AdminUser)
            .AsQueryable();

        if (clienteId.HasValue)
            query = query.Where(e => e.ClienteId == clienteId.Value);
        if (seccionId.HasValue)
            query = query.Where(e => e.SeccionId == seccionId.Value);
        if (tipoCafeId.HasValue)
            query = query.Where(e => e.TipoCafeId == tipoCafeId.Value);
        if (desde.HasValue)
            query = query.Where(e => e.Fecha >= desde.Value);
        if (hasta.HasValue)
            query = query.Where(e => e.Fecha <= hasta.Value);

        return await query.OrderByDescending(e => e.Fecha).ToListAsync();
    }

    public async Task<MovimientoEntrada?> GetByIdAsync(int id)
    {
        return await _context.MovimientosEntrada
            .Include(e => e.Cliente)
            .Include(e => e.Seccion)
            .Include(e => e.TipoCafe)
            .Include(e => e.AdminUser)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<MovimientoEntrada> AddAsync(MovimientoEntrada entrada)
    {
        _context.MovimientosEntrada.Add(entrada);
        await _context.SaveChangesAsync();
        return entrada;
    }
}