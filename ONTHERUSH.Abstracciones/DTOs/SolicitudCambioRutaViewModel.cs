using System.ComponentModel.DataAnnotations;

namespace ONTHERUSH.Abstracciones.DTOs
{
    public class SolicitudCambioRutaViewModel
    {
        public int ViajeId { get; set; }

        public string Ruta { get; set; } = string.Empty;

        public DateTime Salida { get; set; }

        public string Estado { get; set; } = string.Empty;

        public int Pasajeros { get; set; }

        public int TotalParadas { get; set; }

        public int Pendientes { get; set; }

        public int Recogidos { get; set; }

        public string DestinoFinal { get; set; } = string.Empty;

        public List<ParadaAsignadaDto> Paradas { get; set; } = new();

        [Required(ErrorMessage = "Debe ingresar el motivo de la solicitud.")]
        public string Motivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe describir el cambio solicitado.")]
        public string DescripcionCambio { get; set; } = string.Empty;

        public bool HayCambios { get; set; }
    }
}