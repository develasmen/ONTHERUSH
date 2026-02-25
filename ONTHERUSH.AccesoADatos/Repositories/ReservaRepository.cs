using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ONTHERUSH.AccesoADatos.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoOperacion> CrearReserva(object reservaObj)
        {
            var reserva = (Reserva)reservaObj;

            try
            {
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Reserva creada exitosamente",
                    Datos = reserva
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al crear la reserva: {ex.Message}"
                };
            }
        }

        public async Task<List<object>> ObtenerReservasPendientes()
        {
            return _context.Reservas
                .Where(r => r.Estado == "Pendiente")
                .OrderBy(r => r.FechaReserva)
                .Cast<object>()
                .ToList();
        }

        public async Task<object?> ObtenerReservaPorId(int reservaId)
        {
            return await _context.Reservas
                .FirstOrDefaultAsync(r => r.ReservaId == reservaId);
        }

        public async Task<ResultadoOperacion> ActualizarReserva(object reservaObj)
        {
            var reserva = (Reserva)reservaObj;

            try
            {
                _context.Reservas.Update(reserva);
                await _context.SaveChangesAsync();

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Reserva actualizada correctamente",
                    Datos = reserva
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al actualizar reserva: {ex.Message}"
                };
            }
        }
        public async Task<List<object>> ObtenerReservasAsignadasPorConductor(int conductorId)
        {
            var reservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Viaje)
                .Where(r => r.ViajeId != null
                         && r.Viaje != null
                         && r.Viaje.ConductorId == conductorId
                         && (r.Estado == "Asignada" || r.Estado == "Recogido")
                         && r.Viaje.Estado != "Finalizado")
                .OrderBy(r => r.ReservaId)
                .ToListAsync();

            return reservas.Cast<object>().ToList();
        }
    }
}