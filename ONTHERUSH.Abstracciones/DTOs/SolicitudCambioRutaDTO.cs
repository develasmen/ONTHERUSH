using System.ComponentModel.DataAnnotations;

namespace ONTHERUSH.Abstracciones.DTOs
{
    public class SolicitudCambioRutaDTO
    {
        public int ViajeId { get; set; }

        public string Ruta { get; set; } = string.Empty;

        public DateTime Salida { get; set; }

        public string Estado { get; set; } = string.Empty;

        public int Pasajeros { get; set; }

        [Required(ErrorMessage = "Debe ingresar el motivo de la solicitud.")]
        public string Motivo { get; set; } = string.Empty;

        public bool HayCambios { get; set; }
    }
}