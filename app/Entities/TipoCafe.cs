namespace MiApi.Entities;

public class TipoCafe
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;  // Arábica, Robusta, Liofilizado...
    public string? Descripcion { get; set; }
}