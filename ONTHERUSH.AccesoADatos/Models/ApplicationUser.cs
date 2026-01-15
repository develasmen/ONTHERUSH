using Microsoft.AspNetCore.Identity;

namespace ONTHERUSH.AccesoADatos.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Estado { get; set; } = true;
        public DateTime? FechaUltimaModificacion { get; set; }
    }
}