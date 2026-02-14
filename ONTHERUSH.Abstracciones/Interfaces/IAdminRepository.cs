using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<object>> ObtenerTodosLosUsuarios();
        Task<ResultadoOperacion> AsignarRolAUsuario(object usuario, string rol);
        Task<bool> RolExiste(string rol);
        Task<ResultadoOperacion> CrearAdministrador(string userId);
        Task<ResultadoOperacion> CrearConductor(string userId);
    }
}

