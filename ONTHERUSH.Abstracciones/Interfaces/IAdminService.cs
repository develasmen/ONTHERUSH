using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IAdminService
    {
        Task<List<object>> ObtenerUsuariosSinRol();
        Task<ResultadoOperacion> AsignarRol(string userId, string rol);
        Task<ResultadoOperacion> RechazarUsuario(string userId);
        Task<object?> ObtenerConductorPorUserId(string userId);
        Task<object?> ObtenerAdministradorPorUserId(string userId);
    }
}