using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class SolicitudCambioRutaService : ISolicitudCambioRutaService
    {
        private readonly ISolicitudCambioRutaRepository _repository;

        public SolicitudCambioRutaService(ISolicitudCambioRutaRepository repository)
        {
            _repository = repository;
        }

        public async Task CrearSolicitud(SolicitudCambioRutaDTO dto)
        {
            dto.Estado = "Pendiente";
            dto.FechaSolicitud = DateTime.Now;

            await _repository.CrearSolicitud(dto);
        }

        public async Task<List<SolicitudCambioRutaDTO>> ObtenerSolicitudesPendientes()
        {
            return await _repository.ObtenerSolicitudesPendientes();
        }

        public async Task<SolicitudCambioRutaDTO?> ObtenerPorId(int solicitudId)
        {
            return await _repository.ObtenerPorId(solicitudId);
        }

        public async Task AprobarSolicitud(int solicitudId)
        {
            await _repository.AprobarSolicitud(solicitudId);
        }

        public async Task RechazarSolicitud(int solicitudId, string comentarioAdministrador)
        {
            await _repository.RechazarSolicitud(solicitudId, comentarioAdministrador);
        }
    }
}