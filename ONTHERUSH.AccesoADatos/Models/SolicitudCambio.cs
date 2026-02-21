using System.ComponentModel.DataAnnotations;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class SolicitudCambio
    {
        [Key]
        public int SolicitudCambioId { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        public virtual ApplicationUser? Usuario { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string TipoCambio { get; set; } = string.Empty; // Email o Direccion

        [Required]
        [MaxLength(200)]
        public string ValorActual { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ValorNuevo { get; set; } = string.Empty;

        // Estado
        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobada, Rechazada

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public DateTime? FechaRespuesta { get; set; }

        [MaxLength(500)]
        public string? MotivoRechazo { get; set; }
    }
}