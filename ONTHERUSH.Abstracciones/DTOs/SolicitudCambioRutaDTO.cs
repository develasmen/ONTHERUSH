namespace ONTHERUSH.Abstracciones.DTOs
{
    public class SolicitudCambioRutaDTO
    {
        public int SolicitudCambioRutaId { get; set; }
        public int ViajeId { get; set; }
        public int ConductorId { get; set; }
        public string RutaActual { get; set; } = string.Empty;
        public string NuevoOrdenPropuesto { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string NombreConductor { get; set; } = string.Empty;
    }
}