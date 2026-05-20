using MiApi.Entities;
using MiApi.Repositories;

namespace MiApi.Services;

public class TipoCafeService : ITipoCafeService
{
    private readonly ITipoCafeRepository _repository;

    public TipoCafeService(ITipoCafeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TipoCafe>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TipoCafe?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<TipoCafe> CreateAsync(TipoCafe tipoCafe)
    {
        if (await _repository.ExisteNombreAsync(tipoCafe.Nombre))
            throw new InvalidOperationException($"Ya existe un tipo de café con el nombre '{tipoCafe.Nombre}'.");

        return await _repository.AddAsync(tipoCafe);
    }

    public async Task UpdateAsync(int id, TipoCafe tipoCafe)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró el tipo de café con ID {id}.");

        if (await _repository.ExisteNombreAsync(tipoCafe.Nombre, excludeId: id))
            throw new InvalidOperationException($"Ya existe otro tipo de café con el nombre '{tipoCafe.Nombre}'.");

        existing.Nombre = tipoCafe.Nombre;
        existing.Descripcion = tipoCafe.Descripcion;

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var tipoCafe = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró el tipo de café con ID {id}.");

        // Opcional: validar que no esté siendo usado en inventario o movimientos
        await _repository.DeleteAsync(id);
    }
}