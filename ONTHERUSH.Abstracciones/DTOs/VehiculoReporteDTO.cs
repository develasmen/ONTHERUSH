namespace ONTHERUSH.Abstracciones.DTOs
{
    public class VehiculoReporteDTO
    {
        public string Placa { get; set; }
        public string MarcaModelo { get; set; }
        public int Annio { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
