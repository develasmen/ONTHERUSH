using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONTHERUSH.Abstracciones.DTOs
{
    public class UsuarioReporteDTO
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoUsuario { get; set; }
    }
}
