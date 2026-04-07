using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface ISolicitudCambioRutaRepository
    {
        Task CrearSolicitud(SolicitudCambioRutaDTO solicitudDto);
        Task<List<SolicitudCambioRutaDTO>> ObtenerSolicitudesPendientes();
        Task<SolicitudCambioRutaDTO?> ObtenerPorId(int solicitudId);
        Task AprobarSolicitud(int solicitudId);
        Task RechazarSolicitud(int solicitudId, string comentarioAdministrador);
    }
}