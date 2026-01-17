using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class Auditoria
    {
        [Key]
        public int AuditoriaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Tabla { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Accion { get; set; } = string.Empty;

        public string? UserId { get; set; }

        public int? RegistroId { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Descripcion { get; set; }



        [ForeignKey("UserId")]
        public ApplicationUser? Usuario { get; set; }
    }
}