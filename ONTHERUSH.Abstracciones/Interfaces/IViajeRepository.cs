using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IViajeRepository
    {
        Task<ResultadoOperacion> CrearViaje(object viaje);
        Task<object?> ObtenerViajePorId(int viajeId);
        Task<ResultadoOperacion> FinalizarViaje(int viajeId);
        Task<List<ReporteViajeDTO>> ObtenerHistorialViajesPorConductor(int conductorId);
        Task<DetalleViajeDTO?> ObtenerDetalleViajePorConductor(int viajeId, int conductorId);
        Task<ResultadoOperacion> ActualizarEstadoViaje(int viajeId, string nuevoEstado);
    }
}