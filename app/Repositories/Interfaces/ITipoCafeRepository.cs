using MiApi.Entities;

namespace MiApi.Repositories;

public interface ITipoCafeRepository
{
    Task<IEnumerable<TipoCafe>> GetAllAsync();
    Task<TipoCafe?> GetByIdAsync(int id);
    Task<TipoCafe> AddAsync(TipoCafe tipoCafe);
    Task UpdateAsync(TipoCafe tipoCafe);
    Task DeleteAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
}