using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IReservaService
    {
        Task<ResultadoOperacion> CrearReserva(string userId, TimeSpan horaDestino, string provincia, string canton, string distrito);
    }
}