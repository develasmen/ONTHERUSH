using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.AccesoADatos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class ReporteViajesService
    {
        private readonly ApplicationDbContext _context;

        public ReporteViajesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ReporteViajeDTO> ObtenerReporte(DateTime inicio, DateTime fin)
        {
            return _context.Viajes
                .Where(v => v.FechaHoraSalida.Date >= inicio.Date &&
                            v.FechaHoraSalida.Date <= fin.Date)
                .Select(v => new ReporteViajeDTO
                {
                    Ruta = v.NombreRuta,
                    Salida = v.FechaHoraSalida,
                    Llegada = v.HoraEstimadaLlegada,
                    Conductor = v.Conductor.User.Nombre + " " + v.Conductor.User.Apellido,
                    Vehiculo = v.Vehiculo.Marca + " " + v.Vehiculo.Modelo,
                    Estado = v.Estado,
                    Pasajeros = v.CantidadPasajeros
                })
                .OrderBy(v => v.Salida)
                .ToList();
        }
    }
}
