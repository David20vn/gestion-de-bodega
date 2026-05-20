using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Entities;

namespace MiApi.Repositories;

public class InventarioRepository : IInventarioRepository
{
    private readonly AppDbContext _context;

    public InventarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Inventario>> GetAllAsync()
    {
        return await _context.Inventarios
            .Include(i => i.Seccion)
            .Include(i => i.TipoCafe)
            .OrderBy(i => i.Seccion.Nombre)
                .ThenBy(i => i.TipoCafe.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inventario>> GetBySeccionAsync(int seccionId)
    {
        return await _context.Inventarios
            .Include(i => i.Seccion)
            .Include(i => i.TipoCafe)
            .Where(i => i.SeccionId == seccionId)
            .OrderBy(i => i.TipoCafe.Nombre)
            .ToListAsync();
    }

    public async Task<Inventario?> GetBySeccionAndTipoAsync(int seccionId, int tipoCafeId)
    {
        return await _context.Inventarios
            .FirstOrDefaultAsync(i => i.SeccionId == seccionId && i.TipoCafeId == tipoCafeId);
    }

    public async Task<Inventario> AddAsync(Inventario inventario)
    {
        _context.Inventarios.Add(inventario);
        await _context.SaveChangesAsync();
        return inventario;
    }

    public async Task UpdateAsync(Inventario inventario)
    {
        _context.Inventarios.Update(inventario);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetStockAsync(int seccionId, int tipoCafeId)
    {
        var inventario = await _context.Inventarios
            .FirstOrDefaultAsync(i => i.SeccionId == seccionId && i.TipoCafeId == tipoCafeId);
        return inventario?.Cantidad ?? 0;
    }
}