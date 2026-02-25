using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IViajeRepository
    {
        Task<ResultadoOperacion> CrearViaje(object viaje);
        Task<object?> ObtenerViajePorId(int viajeId);
        Task<ResultadoOperacion> FinalizarViaje(int viajeId);
    }
}