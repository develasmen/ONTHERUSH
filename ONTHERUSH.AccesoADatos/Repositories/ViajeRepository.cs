using Microsoft.EntityFrameworkCore;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.AccesoADatos.Repositories
{
    public class ViajeRepository : IViajeRepository
    {
        private readonly ApplicationDbContext _context;

        public ViajeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoOperacion> CrearViaje(object viaje)
        {
            try
            {
                if (viaje is not Viaje v)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje = "El objeto enviado no es un Viaje válido."
                    };
                }

                _context.Viajes.Add(v);
                await _context.SaveChangesAsync();

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Viaje creado correctamente.",
                    Datos = v.ViajeId
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al crear viaje: {ex.Message}"
                };
            }
        }

        public async Task<object?> ObtenerViajePorId(int viajeId)
        {
            return await _context.Viajes
                .Include(x => x.Reservas)
                .Include(x => x.Vehiculo)
                .FirstOrDefaultAsync(x => x.ViajeId == viajeId);
        }

        public async Task<ResultadoOperacion> FinalizarViaje(int viajeId)
        {
            var viaje = await _context.Viajes
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Viaje no encontrado."
                };
            }

            viaje.Estado = "Finalizado";

            await _context.SaveChangesAsync();

            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = "Viaje finalizado correctamente."
            };
        }
        public async Task<ResultadoOperacion> ActualizarEstadoViaje(int viajeId, string nuevoEstado)
        {
            var viaje = await _context.Viajes
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Viaje no encontrado."
                };
            }

            viaje.Estado = nuevoEstado;
            viaje.FechaUltimaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = "Estado del viaje actualizado correctamente."
            };
        }

        public async Task<List<ReporteViajeDTO>> ObtenerHistorialViajesPorConductor(int conductorId)
        {
            return await _context.Viajes
                .Include(v => v.Conductor)
                    .ThenInclude(c => c.User)
                .Include(v => v.Vehiculo)
                .Where(v => v.ConductorId == conductorId)
                .OrderByDescending(v => v.FechaHoraSalida)
                .Select(v => new ReporteViajeDTO
                {
                    ViajeId = v.ViajeId,
                    Ruta = string.IsNullOrWhiteSpace(v.NombreRuta) ? "Ruta no definida" : v.NombreRuta,
                    Salida = v.FechaHoraSalida,
                    Llegada = v.HoraEstimadaLlegada,
                    Conductor = v.Conductor != null && v.Conductor.User != null
                        ? v.Conductor.User.Nombre + " " + v.Conductor.User.Apellido
                        : "Conductor no asignado",
                    Vehiculo = v.Vehiculo != null
                        ? v.Vehiculo.Placa
                        : "Vehículo no asignado",
                    Estado = string.IsNullOrWhiteSpace(v.Estado) ? "Sin estado" : v.Estado,
                    Pasajeros = v.CantidadPasajeros
                })
                .ToListAsync();
        }

        public async Task<DetalleViajeDTO?> ObtenerDetalleViajePorConductor(int viajeId, int conductorId)
        {
            return await _context.Viajes
                .Include(v => v.Conductor)
                    .ThenInclude(c => c.User)
                .Include(v => v.Vehiculo)
                .Where(v => v.ViajeId == viajeId && v.ConductorId == conductorId)
                .Select(v => new DetalleViajeDTO
                {
                    ViajeId = v.ViajeId,
                    Ruta = string.IsNullOrWhiteSpace(v.NombreRuta) ? "Ruta no definida" : v.NombreRuta,
                    Salida = v.FechaHoraSalida,
                    Llegada = v.HoraEstimadaLlegada,
                    Conductor = v.Conductor != null && v.Conductor.User != null
                        ? v.Conductor.User.Nombre + " " + v.Conductor.User.Apellido
                        : "Conductor no asignado",
                    Vehiculo = v.Vehiculo != null
                        ? v.Vehiculo.Placa
                        : "Vehículo no asignado",
                    Estado = string.IsNullOrWhiteSpace(v.Estado) ? "Sin estado" : v.Estado,
                    Pasajeros = v.CantidadPasajeros,
                    Observaciones = string.IsNullOrWhiteSpace(v.Observaciones)
                        ? "Sin observaciones"
                        : v.Observaciones
                })
                .FirstOrDefaultAsync();
        }
    }
}