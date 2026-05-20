namespace MiApi.DTOs;

public class InventarioResponseDto
{
    public int Id { get; set; }
    public int SeccionId { get; set; }
    public string SeccionNombre { get; set; } = string.Empty;
    public int TipoCafeId { get; set; }
    public string TipoCafeNombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
}