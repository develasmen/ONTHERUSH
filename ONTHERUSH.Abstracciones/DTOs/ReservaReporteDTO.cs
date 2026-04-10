using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONTHERUSH.Abstracciones.DTOs
{
    public class ReservaReporteDTO
    {
        public int ReservaId { get; set; }
        public string Cliente { get; set; }
        public string Ruta { get; set; }
        public DateTime FechaReserva { get; set; }
        public string Estado { get; set; }
    }
}
