namespace MiApi.Entities;

public class AuditoriaOperacion
{
    public int Id { get; set; }
    public int AdminUserId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string EntidadAfectada { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? Detalles { get; set; }

    public AdminUser AdminUser { get; set; } = null!;
}