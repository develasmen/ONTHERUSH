using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            EmailService emailService,
            IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            _config = config;
        }

        public async Task<ResultadoOperacion> RegistrarUsuario(RegistroUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Cedula) ||
                string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Apellido) ||
                string.IsNullOrWhiteSpace(dto.Correo) ||
                string.IsNullOrWhiteSpace(dto.Ubicacion) ||
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
            var tokenUrl = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var baseUrl = _config["AppSettings:BaseUrl"]?.TrimEnd('/');

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "No se ha configurado la URL base de la aplicación."
                };
            }

            var link = $"{baseUrl}/Auth/RestablecerContrasena?email={correo}&token={tokenUrl}";

            var asunto = $"Restablecimiento de contraseña - ONTHERUSH - {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            var cuerpo = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 30px;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.08);'>
                    <h2 style='color: #1f2937; margin-bottom: 20px;'>Recuperación de contraseña</h2>

                    <p style='color: #374151; font-size: 15px; line-height: 1.6;'>
                        Hola,
                    </p>

                    <p style='color: #374151; font-size: 15px; line-height: 1.6;'>
                        Recibimos una solicitud para restablecer la contraseña de tu cuenta en <strong>ONTHERUSH</strong>.
                    </p>

                    <p style='color: #374151; font-size: 15px; line-height: 1.6;'>
                        Para continuar, haz clic en el siguiente botón:
                    </p>

                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #2563eb; color: #ffffff; text-decoration: none; padding: 14px 24px; border-radius: 8px; font-size: 15px; display: inline-block;'>
                            Restablecer contraseña
                        </a>
                    </div>

                    <p style='color: #6b7280; font-size: 14px; line-height: 1.6;'>
                        Si el botón no funciona, copia y pega este enlace en tu navegador:
                    </p>

                    <p style='word-break: break-all; color: #2563eb; font-size: 14px;'>
                        {link}
                    </p>

                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 25px 0;'>

                    <p style='color: #6b7280; font-size: 13px; line-height: 1.6;'>
                        Si no solicitaste este cambio, puedes ignorar este mensaje.
                    </p>

                    <p style='color: #6b7280; font-size: 13px; margin-top: 20px;'>
                        Equipo ONTHERUSH
                    </p>
                </div>
            </div>";

            try
            {
                await _emailService.EnviarAsync(correo, asunto, cuerpo);
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al enviar el correo: {ex.Message}"
                };
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

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
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