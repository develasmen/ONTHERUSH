using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class Reserva
    {
        [Key]
        public int ReservaId { get; set; }

        [Required]
        public int ViajeId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [StringLength(300)]
        public string? ParadaDestino { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Activa";

        public DateTime FechaReserva { get; set; } = DateTime.Now;

        public DateTime? FechaCancelacion { get; set; }


        [ForeignKey("ViajeId")]
        public Viaje Viaje { get; set; } = null!;

        [ForeignKey("UserId")]
        public ApplicationUser Usuario { get; set; } = null!;
    }
}