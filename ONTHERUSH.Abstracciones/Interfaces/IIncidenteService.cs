using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces.Services
{
    public interface IIncidenteService
    {
        Task CrearIncidenteAsync(string conductorId, CrearIncidenteDto dto);
        Task<List<IncidenteDto>> ObtenerIncidentesPorConductorAsync(string conductorId);
        Task<List<IncidenteDto>> ObtenerTodosLosIncidentesAsync();
    }
}