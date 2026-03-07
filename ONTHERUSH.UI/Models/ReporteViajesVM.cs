using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.UI.Models
{
    public class ReporteViajesVM
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public List<ReporteViajeDTO> Viajes { get; set; } = new();
    }
}