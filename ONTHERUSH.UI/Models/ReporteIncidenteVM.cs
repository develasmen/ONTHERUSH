using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.UI.Models
{
    public class ReporteIncidenteVM
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public List<IncidenteDto> Incidentes { get; set; } = new List<IncidenteDto>();
    }
}