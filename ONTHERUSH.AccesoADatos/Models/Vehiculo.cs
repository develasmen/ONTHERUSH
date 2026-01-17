using System.ComponentModel.DataAnnotations;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class Vehiculo
    {
        [Key]
        public int VehiculoId { get; set; }

        [Required]
        [StringLength(20)]
        public string Placa { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public int Año { get; set; }

        [Required]
        public int CapacidadPasajeros { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Activo";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Por si un vehiculo tiene muchos viajes
        public ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
    }
}