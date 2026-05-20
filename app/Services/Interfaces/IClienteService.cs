using MiApi.Entities;

namespace MiApi.Services;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task UpdateAsync(int id, Cliente cliente);
    Task DeleteAsync(int id);
}