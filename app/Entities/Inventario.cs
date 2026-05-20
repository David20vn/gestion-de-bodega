namespace MiApi.Entities;

public class Inventario
{
    public int Id { get; set; }
    public int SeccionId { get; set; }
    public int TipoCafeId { get; set; }
    public decimal Cantidad { get; set; }

    // Propiedades de navegación
    public SeccionAlmacenamiento Seccion { get; set; } = null!;
    public TipoCafe TipoCafe { get; set; } = null!;
}