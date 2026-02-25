using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IAuthService
    {
        Task<ResultadoOperacion> RegistrarUsuario(RegistroUsuarioDto dto);
        Task<ResultadoOperacion> IniciarSesion(LoginDto dto);
        Task<ResultadoOperacion> RecuperarContrasena(string correo);
        Task<ResultadoOperacion> RestablecerContrasena(string email, string token, string nuevaContrasena, string confirmarContrasena);
        Task CerrarSesion();
    }
}