using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<ResultadoOperacion> CrearUsuario(object usuario, string contrasena);
        Task<object?> ObtenerPorEmail(string email);
        Task<object?> ObtenerPorId(string userId);
        Task<IList<string>> ObtenerRoles(object usuario);
        Task<ResultadoOperacion> IniciarSesion(object usuario, string contrasena);
        Task<string> GenerarTokenRecuperacion(object usuario);
        Task<ResultadoOperacion> RestablecerContrasenaConToken(object usuario, string token, string nuevaContrasena);
    }
}

