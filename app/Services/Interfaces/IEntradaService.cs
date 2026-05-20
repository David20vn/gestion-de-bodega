using MiApi.Entities;

namespace MiApi.Services;

public interface IEntradaService
{
    Task<IEnumerable<MovimientoEntrada>> GetAllAsync(int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta);
    Task<MovimientoEntrada?> GetByIdAsync(int id);
    Task<MovimientoEntrada> RegistrarEntradaAsync(int adminUserId, int clienteId, int seccionId, int tipoCafeId, decimal cantidad, string? notas);
}