using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Data;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;

        public ReporteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ReporteViajeDTO> ObtenerReporteViaje(DateTime inicio, DateTime fin)
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

        public List<UsuarioReporteDTO> ObtenerUsuarios(DateTime fechaInicio, DateTime fechaFin)
        {
            return _context.Users
                .Where(u => u.FechaRegistro >= fechaInicio && u.FechaRegistro <= fechaFin)
                .Select(u => new UsuarioReporteDTO
                {
                    NombreCompleto = u.Nombre + " " + u.Apellido,
                    Email = u.Email,
                    FechaRegistro = u.FechaRegistro,

                    TipoUsuario = _context.Administradores.Any(a => a.UserId == u.Id) ? "Administrador"
                                   : _context.Conductores.Any(c => c.UserId == u.Id) ? "Conductor"
                                   : "Usuario"
                })
                .OrderByDescending(x => x.FechaRegistro)
                .ToList();
        }

        public List<VehiculoReporteDTO> ObtenerVehiculos()
        {
            return _context.Vehiculos
                .Select(v => new VehiculoReporteDTO
                {
                    Placa = v.Placa,
                    MarcaModelo = v.Marca + " " + v.Modelo,
                    Annio = v.Año,
                    Capacidad = v.CapacidadPasajeros,
                    Estado = v.Estado,
                    FechaRegistro = v.FechaRegistro
                })
                .OrderByDescending(v => v.FechaRegistro)
                .ToList();
        }

        public List<ReservaReporteDTO> ObtenerReservas(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var query = _context.Reservas.AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(r => r.FechaReserva >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(r => r.FechaReserva <= fechaFin.Value);

            return query
                .Select(r => new ReservaReporteDTO
                {
                    ReservaId = r.ReservaId,
                    Cliente = r.Usuario.Nombre, 
                    Ruta = r.Viaje.NombreRuta,
                    FechaReserva = r.FechaReserva,
                    Estado = r.Estado
                })
                .OrderByDescending(r => r.FechaReserva)
                .ToList();
        }
    }
}
