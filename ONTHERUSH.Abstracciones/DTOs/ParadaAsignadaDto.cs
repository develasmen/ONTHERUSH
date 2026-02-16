namespace ONTHERUSH.Abstracciones.DTOs
{
    public class ParadaAsignadaDto
    {
        public int Id { get; set; }
        public int Orden { get; set; }
        public string NombreCliente { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string Estado { get; set; } = "Pendiente";
    }
}
