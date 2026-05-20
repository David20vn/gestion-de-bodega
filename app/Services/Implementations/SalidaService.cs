using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Entities;
using MiApi.Repositories;

namespace MiApi.Services;

public class SalidaService : ISalidaService
{
    private readonly AppDbContext _context;
    private readonly ISalidaRepository _salidaRepository;
    private readonly IInventarioService _inventarioService;

    public SalidaService(
        AppDbContext context,
        ISalidaRepository salidaRepository,
        IInventarioService inventarioService)
    {
        _context = context;
        _salidaRepository = salidaRepository;
        _inventarioService = inventarioService;
    }

    public async Task<IEnumerable<MovimientoSalida>> GetAllAsync(
        int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta)
    {
        return await _salidaRepository.GetAllAsync(clienteId, seccionId, tipoCafeId, desde, hasta);
    }

    public async Task<MovimientoSalida?> GetByIdAsync(int id)
    {
        return await _salidaRepository.GetByIdAsync(id);
    }

    public async Task<MovimientoSalida> RegistrarSalidaAsync(
        int adminUserId, int clienteId, int seccionId, int tipoCafeId, decimal cantidad, string? notas)
    {
        if (cantidad <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor que cero.");

        // Validar stock suficiente antes de proceder
        var stockActual = await _inventarioService.ObtenerStockAsync(seccionId, tipoCafeId);
        if (stockActual < cantidad)
            throw new InvalidOperationException(
                $"Stock insuficiente. Stock actual: {stockActual}, cantidad solicitada: {cantidad}.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Crear el movimiento de salida
            var salida = new MovimientoSalida
            {
                ClienteId = clienteId,
                SeccionId = seccionId,
                TipoCafeId = tipoCafeId,
                Cantidad = cantidad,
                Fecha = DateTime.UtcNow,
                AdminUserId = adminUserId,
                Notas = notas
            };

            await _salidaRepository.AddAsync(salida);

            // 2. Ajustar el inventario (restar la cantidad) – delta negativo
            await _inventarioService.AjustarStockAsync(seccionId, tipoCafeId, delta: -cantidad);

            await transaction.CommitAsync();

            return salida;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}