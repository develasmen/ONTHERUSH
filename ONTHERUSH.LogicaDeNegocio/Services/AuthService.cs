using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly EmailService _emailService;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            EmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
        }

        public async Task<ResultadoOperacion> RegistrarUsuario(RegistroUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Cedula) ||
                string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Apellido) ||
                string.IsNullOrWhiteSpace(dto.Correo) ||
                string.IsNullOrWhiteSpace(dto.Contrasena) ||
                string.IsNullOrWhiteSpace(dto.ConfirmarContrasena))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Por favor complete los campos requeridos."
                };
            }

            if (dto.Contrasena != dto.ConfirmarContrasena)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Las contraseñas no coinciden."
                };
            }

            var usuario = new ApplicationUser
            {
                UserName = dto.Correo,
                Email = dto.Correo,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Cedula = dto.Cedula,
                Direccion = dto.Ubicacion,
                EmailConfirmed = true,
                Estado = true
            };

            var resultado = await _usuarioRepository.CrearUsuario(usuario, dto.Contrasena);

            if (resultado.Exito)
            {
                resultado.Mensaje = "Registro exitoso. Su cuenta está pendiente de aprobación por un administrador.";
            }

            return resultado;
        }

        public async Task<ResultadoOperacion> IniciarSesion(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Por favor ingrese su correo y contraseña."
                };
            }

            var usuario = await _usuarioRepository.ObtenerPorEmail(dto.Correo);

            if (usuario == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Credenciales incorrectas."
                };
            }

            var roles = await _usuarioRepository.ObtenerRoles(usuario);
            if (!roles.Any())
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Su cuenta está pendiente de aprobación por un administrador."
                };
            }

            var resultado = await _usuarioRepository.IniciarSesion(usuario, dto.Contrasena);

            if (resultado.Exito)
            {
                resultado.Datos = new { Usuario = usuario, Roles = roles };
            }

            return resultado;
        }

        public async Task<ResultadoOperacion> RecuperarContrasena(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Por favor ingrese su correo."
                };
            }

            var usuario = await _usuarioRepository.ObtenerPorEmail(correo);

            var mensajeGenerico = "Si el correo existe, se enviará un enlace para restablecer la contraseña.";

            if (usuario == null)
            {
                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = mensajeGenerico
                };
            }

            var token = await _usuarioRepository.GenerarTokenRecuperacion(usuario);
            var tokenUrl = System.Net.WebUtility.UrlEncode(token);

            // Aquí el link debería generarse dinámicamente
            // Por ahora usamos un placeholder
            var link = $"https://localhost:5081/Auth/RestablecerContrasena?email={correo}&token={tokenUrl}";

            try
            {
                await _emailService.EnviarAsync(
                    correo,
                    "Restablecer contraseña - ONTHERUSH",
                    $@"
                    <p>Hola,</p>
                    <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                    <p>Haz clic en el siguiente enlace para cambiarla:</p>
                    <p><a href='{link}'>Restablecer contraseña</a></p>
                    <p>Si no solicitaste este cambio, por favor contacta con soporte.</p>
                ");
            }
            catch
            {
                // Silenciar errores de email por seguridad
            }

            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = mensajeGenerico
            };
        }

        public async Task<ResultadoOperacion> RestablecerContrasena(
            string email, 
            string token, 
            string nuevaContrasena, 
            string confirmarContrasena)
        {
            if (string.IsNullOrWhiteSpace(nuevaContrasena) || string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Debe completar ambos campos."
                };
            }

            if (nuevaContrasena != confirmarContrasena)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Las contraseñas no coinciden."
                };
            }

            var usuario = await _usuarioRepository.ObtenerPorEmail(email);

            if (usuario == null)
            {
                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Si el enlace era válido, la contraseña fue actualizada."
                };
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(token);
            var resultado = await _usuarioRepository.RestablecerContrasenaConToken(usuario, decodedToken, nuevaContrasena);

            if (resultado.Exito)
            {
                resultado.Mensaje = "Contraseña actualizada correctamente. Ya puede iniciar sesión.";
            }

            return resultado;
        }

        public Task CerrarSesion()
        {
            return Task.CompletedTask;
        }
    }
}