using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONTHERUSH.Abstracciones.DTOs
{
    public class ReporteViajeDTO
    {
        public string Ruta { get; set; }
        public DateTime Salida { get; set; }
        public DateTime Llegada { get; set; }
        public string Conductor { get; set; }
        public string Vehiculo { get; set; }
        public string Estado { get; set; }
        public int Pasajeros { get; set; }
    }
}
