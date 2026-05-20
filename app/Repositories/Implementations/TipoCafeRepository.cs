using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Entities;

namespace MiApi.Repositories;

public class TipoCafeRepository : ITipoCafeRepository
{
    private readonly AppDbContext _context;

    public TipoCafeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TipoCafe>> GetAllAsync()
    {
        return await _context.TiposCafe
            .OrderBy(t => t.Nombre)
            .ToListAsync();
    }

    public async Task<TipoCafe?> GetByIdAsync(int id)
    {
        return await _context.TiposCafe.FindAsync(id);
    }

    public async Task<TipoCafe> AddAsync(TipoCafe tipoCafe)
    {
        _context.TiposCafe.Add(tipoCafe);
        await _context.SaveChangesAsync();
        return tipoCafe;
    }

    public async Task UpdateAsync(TipoCafe tipoCafe)
    {
        _context.TiposCafe.Update(tipoCafe);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tipoCafe = await _context.TiposCafe.FindAsync(id);
        if (tipoCafe != null)
        {
            _context.TiposCafe.Remove(tipoCafe);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
    {
        var query = _context.TiposCafe.Where(t => t.Nombre == nombre);
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}