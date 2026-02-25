using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface ISolicitudCambioService
    {
        Task<ResultadoOperacion> SolicitarCambioEmail(string userId, string nuevoEmail);
        Task<ResultadoOperacion> SolicitarCambioDireccion(string userId, string nuevaDireccion);
        Task<List<object>> ObtenerSolicitudesPendientes();
        Task<ResultadoOperacion> AprobarSolicitud(int solicitudId);
        Task<ResultadoOperacion> RechazarSolicitud(int solicitudId, string motivoRechazo);
    }
}