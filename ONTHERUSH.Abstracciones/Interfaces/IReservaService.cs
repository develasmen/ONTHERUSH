using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IReservaService
    {
        Task<ResultadoOperacion> CrearReserva(string userId, TimeSpan horaDestino, string provincia, string canton, string distrito);
        Task<List<object>> ObtenerReservasPendientes();
        Task<object?> ObtenerReservaPorId(int reservaId);
        Task<ResultadoOperacion> ActualizarReserva(object reserva);
        Task<List<object>> ObtenerReservasAsignadasPorConductor(int conductorId);
    }
}