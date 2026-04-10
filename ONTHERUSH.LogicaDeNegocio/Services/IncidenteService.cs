using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces.Services;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;
using Microsoft.EntityFrameworkCore;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class IncidenteService : IIncidenteService
    {
        private readonly ApplicationDbContext _context;

        public IncidenteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CrearIncidenteAsync(string conductorId, CrearIncidenteDto dto)
        {
            var incidente = new Incidente
            {
                ConductorId = conductorId,
                Tipo = dto.Tipo,
                Descripcion = dto.Descripcion,
                Fecha = DateTime.Now,
                Estado = "Pendiente"
            };

            await _context.Incidentes.AddAsync(incidente);
            await _context.SaveChangesAsync();
        }

        public async Task<List<IncidenteDto>> ObtenerIncidentesPorConductorAsync(string conductorId)
        {
            return await _context.Incidentes
                .Where(x => x.ConductorId == conductorId)
                .OrderByDescending(x => x.Fecha)
                .Select(x => new IncidenteDto
                {
                    Id = x.Id,
                    ConductorId = x.ConductorId,
                    ViajeId = x.ViajeId,
                    Tipo = x.Tipo,
                    Descripcion = x.Descripcion,
                    Fecha = x.Fecha,
                    Estado = x.Estado
                })
                .ToListAsync();
        }

        public async Task<List<IncidenteDto>> ObtenerTodosLosIncidentesAsync()
        {
            return await _context.Incidentes
                .GroupJoin(
                    _context.Users,
                    incidente => incidente.ConductorId,
                    usuario => usuario.Id,
                    (incidente, usuarios) => new { incidente, usuarios }
                )
                .SelectMany(
                    x => x.usuarios.DefaultIfEmpty(),
                    (x, usuario) => new IncidenteDto
                    {
                        Id = x.incidente.Id,
                        ConductorId = x.incidente.ConductorId,
                        ViajeId = x.incidente.ViajeId,
                        Tipo = x.incidente.Tipo,
                        Descripcion = x.incidente.Descripcion,
                        Fecha = x.incidente.Fecha,
                        Estado = x.incidente.Estado,
                        NombreConductor = usuario != null
                            ? usuario.Nombre + " " + usuario.Apellido
                            : x.incidente.ConductorId
                    }
                )
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();
        }
    }
}