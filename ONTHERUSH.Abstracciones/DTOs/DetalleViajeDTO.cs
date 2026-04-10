namespace ONTHERUSH.Abstracciones.DTOs
{
    public class DetalleViajeDTO
    {
        public int ViajeId { get; set; }
        public string Ruta { get; set; } = string.Empty;
        public DateTime Salida { get; set; }
        public DateTime Llegada { get; set; }
        public string Conductor { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int Pasajeros { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}