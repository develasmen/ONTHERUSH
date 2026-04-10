using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface ISolicitudCambioRutaService
    {
        Task CrearSolicitud(SolicitudCambioRutaDTO dto);
        Task<List<SolicitudCambioRutaDTO>> ObtenerSolicitudesPendientes();
        Task<SolicitudCambioRutaDTO?> ObtenerPorId(int solicitudId);
        Task AprobarSolicitud(int solicitudId);
        Task RechazarSolicitud(int solicitudId, string comentarioAdministrador);
    }
}