using MiApi.Entities;

namespace MiApi.Repositories;

public interface IInventarioRepository
{
    Task<IEnumerable<Inventario>> GetAllAsync();
    Task<IEnumerable<Inventario>> GetBySeccionAsync(int seccionId);
    Task<Inventario?> GetBySeccionAndTipoAsync(int seccionId, int tipoCafeId);
    Task<Inventario> AddAsync(Inventario inventario);
    Task UpdateAsync(Inventario inventario);
    Task<decimal> GetStockAsync(int seccionId, int tipoCafeId);
}