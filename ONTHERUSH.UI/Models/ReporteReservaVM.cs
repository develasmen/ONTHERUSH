using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.UI.Models
{
    public class ReporteReservaVM
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public List<ReservaReporteDTO> Reservas { get; set; } = new();
    }
}
