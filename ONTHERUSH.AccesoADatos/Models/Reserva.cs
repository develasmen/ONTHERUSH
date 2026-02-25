using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class Reserva
    {
        [Key]
        public int ReservaId { get; set; }

        [Required]
        public TimeSpan HoraDestino { get; set; }

        [Required]
        [MaxLength(50)]
        public string Provincia { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Canton { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Distrito { get; set; } = string.Empty;

        public DateTime FechaReserva { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? Usuario { get; set; }

        // Por si Heiner lo ocupa para mostrar el viaje en la reserva
        public int? ViajeId { get; set; }
        public virtual Viaje? Viaje { get; set; }
    }
}