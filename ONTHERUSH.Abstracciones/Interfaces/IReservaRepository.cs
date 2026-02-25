using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IReservaRepository
    {
        Task<ResultadoOperacion> CrearReserva(object reserva);
        Task<List<object>> ObtenerReservasPendientes();
        Task<object?> ObtenerReservaPorId(int reservaId);
        Task<ResultadoOperacion> ActualizarReserva(object reserva);
        Task<List<object>> ObtenerReservasAsignadasPorConductor(int conductorId);
    }
}
