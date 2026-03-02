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
    }
}