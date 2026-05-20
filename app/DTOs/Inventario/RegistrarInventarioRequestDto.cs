namespace MiApi.DTOs;

public class RegistrarInventarioRequestDto
{
    public int SeccionId { get; set; }
    public int TipoCafeId { get; set; }
    public decimal Cantidad { get; set; }
}