namespace MiApi.DTOs;

public class EntradaResponseDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int SeccionId { get; set; }
    public string SeccionNombre { get; set; } = string.Empty;
    public int TipoCafeId { get; set; }
    public string TipoCafeNombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public int AdminUserId { get; set; }
    public string AdminUsername { get; set; } = string.Empty;
    public string? Notas { get; set; }
}