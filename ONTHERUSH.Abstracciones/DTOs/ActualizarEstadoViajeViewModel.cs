using System.ComponentModel.DataAnnotations;

namespace ONTHERUSH.Abstracciones.DTOs
{
    public class ActualizarEstadoViajeViewModel
    {
        public int ViajeId { get; set; }

        public string Ruta { get; set; } = string.Empty;

        public DateTime Salida { get; set; }

        public string EstadoActual { get; set; } = string.Empty;

        public int Pasajeros { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el nuevo estado.")]
        public string NuevoEstado { get; set; } = string.Empty;
    }
}