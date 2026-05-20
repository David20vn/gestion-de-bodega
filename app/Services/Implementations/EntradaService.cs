using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Entities;
using MiApi.Repositories;

namespace MiApi.Services;

public class EntradaService : IEntradaService
{
    private readonly AppDbContext _context; // Necesitamos el contexto para la transacción
    private readonly IEntradaRepository _entradaRepository;
    private readonly IInventarioService _inventarioService;

    public EntradaService(
        AppDbContext context,
        IEntradaRepository entradaRepository,
        IInventarioService inventarioService)
    {
        _context = context;
        _entradaRepository = entradaRepository;
        _inventarioService = inventarioService;
    }

    public async Task<IEnumerable<MovimientoEntrada>> GetAllAsync(
        int? clienteId, int? seccionId, int? tipoCafeId, DateTime? desde, DateTime? hasta)
    {
        return await _entradaRepository.GetAllAsync(clienteId, seccionId, tipoCafeId, desde, hasta);
    }

    public async Task<MovimientoEntrada?> GetByIdAsync(int id)
    {
        return await _entradaRepository.GetByIdAsync(id);
    }

    public async Task<MovimientoEntrada> RegistrarEntradaAsync(
        int adminUserId, int clienteId, int seccionId, int tipoCafeId, decimal cantidad, string? notas)
    {
        if (cantidad <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor que cero.");

        // Usamos una transacción para garantizar que movimiento e inventario se actualicen juntos
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Crear el movimiento de entrada
            var entrada = new MovimientoEntrada
            {
                ClienteId = clienteId,
                SeccionId = seccionId,
                TipoCafeId = tipoCafeId,
                Cantidad = cantidad,
                Fecha = DateTime.UtcNow,
                AdminUserId = adminUserId,
                Notas = notas
            };

            await _entradaRepository.AddAsync(entrada);

            // 2. Ajustar el inventario (sumar la cantidad)
            await _inventarioService.AjustarStockAsync(seccionId, tipoCafeId, delta: cantidad);

            await transaction.CommitAsync();

            return entrada;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}