using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface ISolicitudCambioRepository
    {
        Task<ResultadoOperacion> CrearSolicitud(object solicitud);
        Task<List<object>> ObtenerSolicitudesPendientes();
        Task<object?> ObtenerPorId(int solicitudId);
        Task<ResultadoOperacion> AprobarSolicitud(int solicitudId);
        Task<ResultadoOperacion> RechazarSolicitud(int solicitudId, string motivoRechazo);
    }
}