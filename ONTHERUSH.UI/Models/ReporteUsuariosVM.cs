using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.UI.Models
{
    public class ReporteUsuariosVM
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public List<UsuarioReporteDTO> Usuarios { get; set; } = new();
    }
}
