using MiApi.Entities;

namespace MiApi.Repositories;

public interface ISalidaRepository
{
    Task<IEnumerable<MovimientoSalida>> GetAllAsync(int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta);
    Task<MovimientoSalida?> GetByIdAsync(int id);
    Task<MovimientoSalida> AddAsync(MovimientoSalida salida);
}