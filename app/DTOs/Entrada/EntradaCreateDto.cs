namespace MiApi.DTOs;

public class EntradaCreateDto
{
    public int ClienteId { get; set; }
    public int SeccionId { get; set; }
    public int TipoCafeId { get; set; }
    public decimal Cantidad { get; set; }
    public string? Notas { get; set; }
}