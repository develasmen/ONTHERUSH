using Microsoft.EntityFrameworkCore;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.AccesoADatos.Repositories
{
    public class SolicitudCambioRepository : ISolicitudCambioRepository
    {
        private readonly ApplicationDbContext _context;

        public SolicitudCambioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoOperacion> CrearSolicitud(object solicitudObj)
        {
            var solicitud = (SolicitudCambio)solicitudObj;

            try
            {
                _context.SolicitudesCambio.Add(solicitud);
                await _context.SaveChangesAsync();

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Solicitud creada exitosamente. Será revisada por un administrador.",
                    Datos = solicitud
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al crear la solicitud: {ex.Message}"
                };
            }
        }

        public async Task<List<object>> ObtenerSolicitudesPendientes()
        {
            var solicitudes = await _context.SolicitudesCambio
                .Include(s => s.Usuario)
                .Where(s => s.Estado == "Pendiente")
                .OrderBy(s => s.FechaSolicitud)
                .ToListAsync();

            return solicitudes.Cast<object>().ToList();
        }

        public async Task<object?> ObtenerPorId(int solicitudId)
        {
            return await _context.SolicitudesCambio
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(s => s.SolicitudCambioId == solicitudId);
        }

        public async Task<ResultadoOperacion> AprobarSolicitud(int solicitudId)
        {
            var solicitud = await _context.SolicitudesCambio.FindAsync(solicitudId);

            if (solicitud == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            solicitud.Estado = "Aprobada";
            solicitud.FechaRespuesta = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = "Solicitud aprobada exitosamente",
                Datos = solicitud
            };
        }

        public async Task<ResultadoOperacion> RechazarSolicitud(int solicitudId, string motivoRechazo)
        {
            var solicitud = await _context.SolicitudesCambio.FindAsync(solicitudId);

            if (solicitud == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            solicitud.Estado = "Rechazada";
            solicitud.FechaRespuesta = DateTime.Now;
            solicitud.MotivoRechazo = motivoRechazo;

            await _context.SaveChangesAsync();

            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = "Solicitud rechazada",
                Datos = solicitud
            };
        }
    }
}