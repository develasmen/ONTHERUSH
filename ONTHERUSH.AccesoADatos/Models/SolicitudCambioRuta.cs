using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class SolicitudCambioRuta
    {
        [Key]
        public int SolicitudCambioRutaId { get; set; }

        [Required]
        public int ViajeId { get; set; }

        [Required]
        public int ConductorId { get; set; }

        [Required]
        [StringLength(200)]
        public string RutaActual { get; set; } = string.Empty;

        [Required]
        public string NuevoOrdenPropuesto { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public DateTime? FechaResolucion { get; set; }

        [StringLength(500)]
        public string? ComentarioAdministrador { get; set; }

        [ForeignKey("ViajeId")]
        public Viaje Viaje { get; set; } = null!;

        [ForeignKey("ConductorId")]
        public Conductor Conductor { get; set; } = null!;
    }
}