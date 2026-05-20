namespace MiApi.Entities;

public class SeccionAlmacenamiento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;  // Café en grano, Café molido, etc.
    public string? Descripcion { get; set; }
}