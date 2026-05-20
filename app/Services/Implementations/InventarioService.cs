using MiApi.Entities;
using MiApi.Repositories;

namespace MiApi.Services;

public class InventarioService : IInventarioService
{
    private readonly IInventarioRepository _repository;

    public InventarioService(IInventarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Inventario>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Inventario>> GetBySeccionAsync(int seccionId)
    {
        return await _repository.GetBySeccionAsync(seccionId);
    }

    public async Task<Inventario?> GetBySeccionAndTipoAsync(int seccionId, int tipoCafeId)
    {
        return await _repository.GetBySeccionAndTipoAsync(seccionId, tipoCafeId);
    }

    public async Task<Inventario> RegistrarOActualizarAsync(int seccionId, int tipoCafeId, decimal cantidad)
    {
        if (cantidad < 0)
            throw new InvalidOperationException("La cantidad inicial no puede ser negativa.");

        var existente = await _repository.GetBySeccionAndTipoAsync(seccionId, tipoCafeId);
        if (existente != null)
        {
            existente.Cantidad = cantidad;
            await _repository.UpdateAsync(existente);
            return existente;
        }
        else
        {
            var nuevo = new Inventario
            {
                SeccionId = seccionId,
                TipoCafeId = tipoCafeId,
                Cantidad = cantidad
            };
            return await _repository.AddAsync(nuevo);
        }
    }

    public async Task AjustarStockAsync(int seccionId, int tipoCafeId, decimal delta)
    {
        // delta positivo = entrada, negativo = salida
        var inventario = await _repository.GetBySeccionAndTipoAsync(seccionId, tipoCafeId)
            ?? throw new InvalidOperationException("No existe registro de inventario para esta combinación sección/tipo de café.");

        var nuevoStock = inventario.Cantidad + delta;
        if (nuevoStock < 0)
            throw new InvalidOperationException("Stock insuficiente para realizar la operación.");

        inventario.Cantidad = nuevoStock;
        await _repository.UpdateAsync(inventario);
    }

    public async Task<decimal> ObtenerStockAsync(int seccionId, int tipoCafeId)
    {
        return await _repository.GetStockAsync(seccionId, tipoCafeId);
    }
}