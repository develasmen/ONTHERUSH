using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;

        public ReservaService(IReservaRepository reservaRepository)
        {
            _reservaRepository = reservaRepository;
        }

        public async Task<ResultadoOperacion> CrearReserva(
            string userId,
            TimeSpan horaDestino,
            string provincia,
            string canton,
            string distrito)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Usuario no válido"
                };
            }

            if (string.IsNullOrWhiteSpace(provincia) || string.IsNullOrWhiteSpace(canton) || string.IsNullOrWhiteSpace(distrito))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Todos los campos de ubicación son requeridos"
                };
            }

            // Revisar
            var provinciasValidas = new[] { "San José", "Heredia", "Alajuela" };
            if (!provinciasValidas.Contains(provincia))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Provincia no válida. Solo se aceptan: San José, Heredia, Alajuela"
                };
            }

            // Crear la reserva
            var reserva = new Reserva
            {
                UserId = userId,
                HoraDestino = horaDestino,
                Provincia = provincia,
                Canton = canton,
                Distrito = distrito,
                FechaReserva = DateTime.Now,
                Estado = "Pendiente"
            };

            return await _reservaRepository.CrearReserva(reserva);
        }

        public async Task<List<object>> ObtenerReservasPendientes()
        {
            return await _reservaRepository.ObtenerReservasPendientes();
        }

        public async Task<object?> ObtenerReservaPorId(int reservaId)
        {
            return await _reservaRepository.ObtenerReservaPorId(reservaId);
        }

        public async Task<ResultadoOperacion> ActualizarReserva(object reserva)
        {
            return await _reservaRepository.ActualizarReserva(reserva);
        }

        public async Task<List<object>> ObtenerReservasAsignadasPorConductor(int conductorId)
        {
            return await _reservaRepository.ObtenerReservasAsignadasPorConductor(conductorId);
        }
    }
}