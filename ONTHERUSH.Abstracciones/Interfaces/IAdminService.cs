using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IAdminService
    {
        Task<List<object>> ObtenerUsuariosSinRol();
        Task<ResultadoOperacion> AsignarRol(string userId, string rol);
    }
}