using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class Viaje
    {
        [Key]
        public int ViajeId { get; set; }

        [Required]
        public int ConductorId { get; set; }

        [Required]
        public int VehiculoId { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreRuta { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string PuntoPartida { get; set; } = string.Empty;

        public string? Paradas { get; set; }

        [Required]
        public DateTime FechaHoraSalida { get; set; }

        [Required]
        public DateTime HoraEstimadaLlegada { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Programado";

        public int CantidadPasajeros { get; set; } = 0;

        [Required]
        public int AsignadoPorAdminId { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? Observaciones { get; set; }

        [ForeignKey("ConductorId")]
        public Conductor Conductor { get; set; } = null!;

        [ForeignKey("VehiculoId")]
        public Vehiculo Vehiculo { get; set; } = null!;

        [ForeignKey("AsignadoPorAdminId")]
        public Administrador AsignadoPor { get; set; } = null!;



        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}