using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class Conductor
    {
        [Key]
        public int ConductorId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        public DateTime FechaContratacion { get; set; } = DateTime.Now;

        public DateTime? FechaUltimaModificacion { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        // Por si un conductor tiene muchos viajes
        public ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
    }
}