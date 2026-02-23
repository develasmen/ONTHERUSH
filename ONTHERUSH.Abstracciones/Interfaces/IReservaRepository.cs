using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IReservaRepository
    {
        Task<ResultadoOperacion> CrearReserva(object reserva);
    }
}
