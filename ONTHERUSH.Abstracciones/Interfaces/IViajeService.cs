using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IViajeService
    {
        Task<ResultadoOperacion> CrearViaje(
            int conductorId,
            int asignadoPorAdminId,
            int vehiculoId,
            string nombreRuta,
            string puntoPartida,
            string? paradas,
            DateTime fechaHoraSalida,
            DateTime horaEstimadaLlegada,
            int cantidadPasajeros,
            string? observaciones
        );

        Task<object?> ObtenerViajePorId(int viajeId);
        Task<ResultadoOperacion> FinalizarViaje(int viajeId);
        Task<List<ReporteViajeDTO>> ObtenerHistorialViajesPorConductor(int conductorId);
        Task<DetalleViajeDTO?> ObtenerDetalleViajePorConductor(int viajeId, int conductorId);
        Task<ResultadoOperacion> ActualizarEstadoViaje(int viajeId, string nuevoEstado);
    }
}