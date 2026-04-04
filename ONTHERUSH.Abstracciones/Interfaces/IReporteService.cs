using ONTHERUSH.Abstracciones.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IReporteService
    {
        public List<ReporteViajeDTO> ObtenerReporteViaje(DateTime inicio, DateTime fin);
        public List<UsuarioReporteDTO> ObtenerUsuarios(DateTime fechaInicio, DateTime fechaFin);
    }
}
