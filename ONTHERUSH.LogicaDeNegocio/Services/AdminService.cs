using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public AdminService(
            IAdminRepository adminRepository,
            IUsuarioRepository usuarioRepository)
        {
            _adminRepository = adminRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<object>> ObtenerUsuariosSinRol()
        {
            var todosLosUsuarios = await _adminRepository.ObtenerTodosLosUsuarios();
            var usuariosSinRol = new List<object>();

            foreach (var usuarioObj in todosLosUsuarios)
            {
                var roles = await _usuarioRepository.ObtenerRoles(usuarioObj);
                if (!roles.Any())
                {
                    usuariosSinRol.Add(usuarioObj);
                }
            }

            return usuariosSinRol;
        }

        public async Task<ResultadoOperacion> AsignarRol(string userId, string rol)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(rol))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Datos inválidos."
                };
            }

            var usuarioObj = await _usuarioRepository.ObtenerPorId(userId);
            if (usuarioObj == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Usuario no encontrado."
                };
            }

            if (!await _adminRepository.RolExiste(rol))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Rol no válido."
                };
            }

            var resultado = await _adminRepository.AsignarRolAUsuario(usuarioObj, rol);

            if (!resultado.Exito)
            {
                return resultado;
            }

            if (rol == "Conductor")
            {
                await _adminRepository.CrearConductor(userId);
            }
            else if (rol == "Administrador")
            {
                await _adminRepository.CrearAdministrador(userId);
            }

            var usuario = (ApplicationUser)usuarioObj;
            resultado.Mensaje = $"Rol '{rol}' asignado exitosamente a {usuario.Nombre} {usuario.Apellido}.";
            return resultado;
        }

        public async Task<ResultadoOperacion> RechazarUsuario(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "ID de usuario inválido."
                };
            }

            var usuarioObj = await _usuarioRepository.ObtenerPorId(userId);
            if (usuarioObj == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Usuario no encontrado."
                };
            }

            var usuario = (ApplicationUser)usuarioObj;
            var resultado = await _adminRepository.EliminarUsuario(usuarioObj);

            if (resultado.Exito)
            {
                resultado.Mensaje = $"Usuario {usuario.Nombre} {usuario.Apellido} rechazado y eliminado exitosamente.";
            }

            return resultado;
        }
    }
}