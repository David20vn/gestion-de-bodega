namespace MiApi.Entities;

public class MovimientoSalida
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int SeccionId { get; set; }
    public int TipoCafeId { get; set; }
    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int AdminUserId { get; set; }
    public string? Notas { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public SeccionAlmacenamiento Seccion { get; set; } = null!;
    public TipoCafe TipoCafe { get; set; } = null!;
    public AdminUser AdminUser { get; set; } = null!;
}