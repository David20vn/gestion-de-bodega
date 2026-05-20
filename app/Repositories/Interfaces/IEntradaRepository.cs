using MiApi.Entities;

namespace MiApi.Repositories;

public interface IEntradaRepository
{
    Task<IEnumerable<MovimientoEntrada>> GetAllAsync(int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta);
    Task<MovimientoEntrada?> GetByIdAsync(int id);
    Task<MovimientoEntrada> AddAsync(MovimientoEntrada entrada);
}