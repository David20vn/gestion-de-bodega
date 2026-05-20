using MiApi.Entities;

namespace MiApi.Services;

public interface ISalidaService
{
    Task<IEnumerable<MovimientoSalida>> GetAllAsync(int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta);
    Task<MovimientoSalida?> GetByIdAsync(int id);
    Task<MovimientoSalida> RegistrarSalidaAsync(int adminUserId, int clienteId, int seccionId, int tipoCafeId, decimal cantidad, string? notas);
}