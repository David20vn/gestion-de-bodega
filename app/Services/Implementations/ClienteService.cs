using MiApi.Entities;
using MiApi.Repositories;

namespace MiApi.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        // Validación de nombre único
        if (await _repository.ExisteNombreAsync(cliente.Nombre))
            throw new InvalidOperationException($"Ya existe un cliente con el nombre '{cliente.Nombre}'.");

        cliente.FechaRegistro = DateTime.UtcNow;
        return await _repository.AddAsync(cliente);
    }

    public async Task UpdateAsync(int id, Cliente cliente)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró el cliente con ID {id}.");

        // Validar que el nuevo nombre no esté en uso por otro cliente
        if (await _repository.ExisteNombreAsync(cliente.Nombre, excludeId: id))
            throw new InvalidOperationException($"Ya existe otro cliente con el nombre '{cliente.Nombre}'.");

        // Actualizar campos
        existing.Nombre = cliente.Nombre;
        existing.Contacto = cliente.Contacto;
        existing.Telefono = cliente.Telefono;
        existing.Email = cliente.Email;

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró el cliente con ID {id}.");

        await _repository.DeleteAsync(id);
    }
}