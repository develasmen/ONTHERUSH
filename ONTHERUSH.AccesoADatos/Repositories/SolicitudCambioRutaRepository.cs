using Microsoft.EntityFrameworkCore;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.AccesoADatos.Repositories
{
    public class SolicitudCambioRutaRepository : ISolicitudCambioRutaRepository
    {
        private readonly ApplicationDbContext _context;

        public SolicitudCambioRutaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CrearSolicitud(SolicitudCambioRutaDTO solicitudDto)
        {
            var solicitud = new SolicitudCambioRuta
            {
                ViajeId = solicitudDto.ViajeId,
                ConductorId = solicitudDto.ConductorId,
                RutaActual = solicitudDto.RutaActual,
                NuevoOrdenPropuesto = solicitudDto.NuevoOrdenPropuesto,
                Motivo = solicitudDto.Motivo,
                Estado = string.IsNullOrWhiteSpace(solicitudDto.Estado) ? "Pendiente" : solicitudDto.Estado,
                FechaSolicitud = solicitudDto.FechaSolicitud == default ? DateTime.Now : solicitudDto.FechaSolicitud
            };

            _context.SolicitudesCambioRuta.Add(solicitud);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SolicitudCambioRutaDTO>> ObtenerSolicitudesPendientes()
        {
            return await _context.SolicitudesCambioRuta
                .Include(x => x.Conductor)
                .ThenInclude(x => x.User)
                .Where(x => x.Estado == "Pendiente")
                .OrderByDescending(x => x.FechaSolicitud)
                .Select(x => new SolicitudCambioRutaDTO
                {
                    SolicitudCambioRutaId = x.SolicitudCambioRutaId,
                    ViajeId = x.ViajeId,
                    ConductorId = x.ConductorId,
                    RutaActual = x.RutaActual,
                    NuevoOrdenPropuesto = x.NuevoOrdenPropuesto,
                    Motivo = x.Motivo,
                    Estado = x.Estado,
                    FechaSolicitud = x.FechaSolicitud,
                    NombreConductor = x.Conductor.User.Nombre + " " + x.Conductor.User.Apellido
                })
                .ToListAsync();
        }

        public async Task<SolicitudCambioRutaDTO?> ObtenerPorId(int solicitudId)
        {
            return await _context.SolicitudesCambioRuta
                .Include(x => x.Conductor)
                .ThenInclude(x => x.User)
                .Where(x => x.SolicitudCambioRutaId == solicitudId)
                .Select(x => new SolicitudCambioRutaDTO
                {
                    SolicitudCambioRutaId = x.SolicitudCambioRutaId,
                    ViajeId = x.ViajeId,
                    ConductorId = x.ConductorId,
                    RutaActual = x.RutaActual,
                    NuevoOrdenPropuesto = x.NuevoOrdenPropuesto,
                    Motivo = x.Motivo,
                    Estado = x.Estado,
                    FechaSolicitud = x.FechaSolicitud,
                    NombreConductor = x.Conductor.User.Nombre + " " + x.Conductor.User.Apellido
                })
                .FirstOrDefaultAsync();
        }

        public async Task AprobarSolicitud(int solicitudId)
        {
            var solicitud = await _context.SolicitudesCambioRuta
                .FirstOrDefaultAsync(x => x.SolicitudCambioRutaId == solicitudId);

            if (solicitud != null)
            {
                solicitud.Estado = "Aprobada";
                solicitud.FechaResolucion = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RechazarSolicitud(int solicitudId, string comentarioAdministrador)
        {
            var solicitud = await _context.SolicitudesCambioRuta
                .FirstOrDefaultAsync(x => x.SolicitudCambioRutaId == solicitudId);

            if (solicitud != null)
            {
                solicitud.Estado = "Rechazada";
                solicitud.ComentarioAdministrador = comentarioAdministrador;
                solicitud.FechaResolucion = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}