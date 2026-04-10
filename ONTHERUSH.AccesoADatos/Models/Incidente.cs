public class Incidente
{
    public int Id { get; set; }

    public string ConductorId { get; set; }

    public int? ViajeId { get; set; }

    public string Tipo { get; set; }

    public string Descripcion { get; set; }

    public DateTime Fecha { get; set; }

    public string Estado { get; set; }
}