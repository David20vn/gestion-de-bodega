using MiApi.Entities;

namespace MiApi.Services;

public interface IInventarioService
{
    Task<IEnumerable<Inventario>> GetAllAsync();
    Task<IEnumerable<Inventario>> GetBySeccionAsync(int seccionId);
    Task<Inventario?> GetBySeccionAndTipoAsync(int seccionId, int tipoCafeId);
    Task<Inventario> RegistrarOActualizarAsync(int seccionId, int tipoCafeId, decimal cantidad);
    Task AjustarStockAsync(int seccionId, int tipoCafeId, decimal delta);
    Task<decimal> ObtenerStockAsync(int seccionId, int tipoCafeId);
}