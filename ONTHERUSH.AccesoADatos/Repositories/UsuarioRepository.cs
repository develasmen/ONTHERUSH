using Microsoft.AspNetCore.Identity;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.AccesoADatos.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsuarioRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ResultadoOperacion> CrearUsuario(object usuario, string contrasena)
        {
            var user = (ApplicationUser)usuario;
            var result = await _userManager.CreateAsync(user, contrasena);

            if (result.Succeeded)
            {
                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Usuario creado exitosamente",
                    Datos = user
                };
            }

            var errores = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ResultadoOperacion
            {
                Exito = false,
                Mensaje = errores
            };
        }

        public async Task<object?> ObtenerPorEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<object?> ObtenerPorId(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IList<string>> ObtenerRoles(object usuario)
        {
            var user = (ApplicationUser)usuario;
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<ResultadoOperacion> IniciarSesion(object usuario, string contrasena)
        {
            var user = (ApplicationUser)usuario;
            var result = await _signInManager.PasswordSignInAsync(
                user,
                contrasena,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Inicio de sesión exitoso"
                };
            }

            return new ResultadoOperacion
            {
                Exito = false,
                Mensaje = "Credenciales incorrectas"
            };
        }

        public async Task<string> GenerarTokenRecuperacion(object usuario)
        {
            var user = (ApplicationUser)usuario;
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ResultadoOperacion> RestablecerContrasenaConToken(
            object usuario,
            string token,
            string nuevaContrasena)
        {
            var user = (ApplicationUser)usuario;
            var result = await _userManager.ResetPasswordAsync(user, token, nuevaContrasena);

            if (result.Succeeded)
            {
                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Contraseña actualizada correctamente"
                };
            }

            var errores = string.Join(" | ", result.Errors.Select(e => e.Description));
            return new ResultadoOperacion
            {
                Exito = false,
                Mensaje = errores
            };
        }
    }
}