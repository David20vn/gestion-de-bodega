using MiApi.Entities;

namespace MiApi.Services;

public interface ITipoCafeService
{
    Task<IEnumerable<TipoCafe>> GetAllAsync();
    Task<TipoCafe?> GetByIdAsync(int id);
    Task<TipoCafe> CreateAsync(TipoCafe tipoCafe);
    Task UpdateAsync(int id, TipoCafe tipoCafe);
    Task DeleteAsync(int id);
}