using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class SolicitudCambioService : ISolicitudCambioService
    {
        private readonly ISolicitudCambioRepository _solicitudRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SolicitudCambioService(
            ISolicitudCambioRepository solicitudRepository,
            IUsuarioRepository usuarioRepository,
            UserManager<ApplicationUser> userManager)
        {
            _solicitudRepository = solicitudRepository;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }

        public async Task<ResultadoOperacion> SolicitarCambioEmail(string userId, string nuevoEmail)
        {
            nuevoEmail = nuevoEmail?.Trim() ?? string.Empty;

            if (!EsCorreoValido(nuevoEmail))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Ingrese un correo electrónico válido. Ejemplo: usuario@dominio.com"
                };
            }

            var usuarioObj = await _usuarioRepository.ObtenerPorId(userId);

            if (usuarioObj == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Usuario no encontrado"
                };
            }

            var usuario = (ApplicationUser)usuarioObj;

            if (string.Equals(
                usuario.Email,
                nuevoEmail,
                StringComparison.OrdinalIgnoreCase))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "El nuevo correo es igual al correo actual"
                };
            }

            var usuarioConMismoCorreo =
                await _userManager.FindByEmailAsync(nuevoEmail);

            if (usuarioConMismoCorreo != null &&
                usuarioConMismoCorreo.Id != userId)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "El correo electrónico ya está registrado por otro usuario"
                };
            }

            var solicitud = new SolicitudCambio
            {
                UsuarioId = userId,
                TipoCambio = "Email",
                ValorActual = usuario.Email ?? string.Empty,
                ValorNuevo = nuevoEmail,
                Estado = "Pendiente",
                FechaSolicitud = DateTime.Now
            };

            return await _solicitudRepository.CrearSolicitud(solicitud);
        }
        public async Task<ResultadoOperacion> SolicitarCambioDireccion(string userId, string nuevaDireccion)
        {
            if (!TryNormalizarCoordenadas(
                nuevaDireccion,
                out var coordenadasNormalizadas))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Ingrese coordenadas válidas en el formato latitud, longitud. Ejemplo: 9.998514, -84.205716"
                };
            }

            var usuarioObj = await _usuarioRepository.ObtenerPorId(userId);

            if (usuarioObj == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Usuario no encontrado"
                };
            }

            var usuario = (ApplicationUser)usuarioObj;

            var solicitud = new SolicitudCambio
            {
                UsuarioId = userId,
                TipoCambio = "Direccion",
                ValorActual = usuario.Direccion ?? string.Empty,
                ValorNuevo = coordenadasNormalizadas,
                Estado = "Pendiente",
                FechaSolicitud = DateTime.Now
            };

            return await _solicitudRepository.CrearSolicitud(solicitud);
        }

        public async Task<List<object>> ObtenerSolicitudesPendientes()
        {
            return await _solicitudRepository.ObtenerSolicitudesPendientes();
        }

        public async Task<ResultadoOperacion> AprobarSolicitud(int solicitudId)
        {
            var solicitudObj = await _solicitudRepository.ObtenerPorId(solicitudId);
            if (solicitudObj == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            var solicitud = (SolicitudCambio)solicitudObj;

            // Aprobar en la tabla SolicitudesCambio
            var resultado = await _solicitudRepository.AprobarSolicitud(solicitudId);

            if (!resultado.Exito)
            {
                return resultado;
            }

            // Aplicamos el cambio a los usuarios
            var usuario = await _userManager.FindByIdAsync(solicitud.UsuarioId);
            if (usuario == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Usuario no encontrado"
                };
            }

            if (solicitud.TipoCambio == "Email")
            {
                usuario.Email = solicitud.ValorNuevo;
                usuario.UserName = solicitud.ValorNuevo;
            }
            else if (solicitud.TipoCambio == "Direccion")
            {
                usuario.Direccion = solicitud.ValorNuevo;
            }

            usuario.FechaUltimaModificacion = DateTime.Now;

            var updateResult = await _userManager.UpdateAsync(usuario);

            if (updateResult.Succeeded)
            {
                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = $"Solicitud aprobada y {solicitud.TipoCambio} actualizado exitosamente"
                };
            }

            return new ResultadoOperacion
            {
                Exito = false,
                Mensaje = "Error al actualizar el usuario"
            };
        }

        public async Task<ResultadoOperacion> RechazarSolicitud(int solicitudId, string motivoRechazo)
        {
            return await _solicitudRepository.RechazarSolicitud(solicitudId, motivoRechazo);
        }

        private static bool EsCorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo) ||
                correo.Length > 256 ||
                correo.Any(char.IsWhiteSpace))
            {
                return false;
            }

            if (!MailAddress.TryCreate(correo, out var direccion) ||
                !string.Equals(
                    direccion.Address,
                    correo,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var posicionArroba = correo.LastIndexOf('@');

            if (posicionArroba <= 0 ||
                posicionArroba == correo.Length - 1)
            {
                return false;
            }

            var dominio = correo[(posicionArroba + 1)..];
            var partesDominio = dominio.Split('.');

            if (partesDominio.Length < 2)
            {
                return false;
            }

            foreach (var parte in partesDominio)
            {
                if (string.IsNullOrWhiteSpace(parte) ||
                    parte.StartsWith('-') ||
                    parte.EndsWith('-') ||
                    !Regex.IsMatch(parte, "^[A-Za-z0-9-]+$"))
                {
                    return false;
                }
            }

            return Regex.IsMatch(
                partesDominio[^1],
                "^[A-Za-z]{2,63}$");
        }

        private static bool TryNormalizarCoordenadas(string coordenadas, out string resultado)
        {
            resultado = string.Empty;

            if (string.IsNullOrWhiteSpace(coordenadas))
            {
                return false;
            }

            var partes = coordenadas.Split(',');

            const NumberStyles formatoNumero =
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowDecimalPoint;

            if (partes.Length != 2 ||
                !decimal.TryParse(
                    partes[0].Trim(),
                    formatoNumero,
                    CultureInfo.InvariantCulture,
                    out var latitud) ||
                !decimal.TryParse(
                    partes[1].Trim(),
                    formatoNumero,
                    CultureInfo.InvariantCulture,
                    out var longitud))
            {
                return false;
            }

            if (latitud < -90 || latitud > 90 ||
                longitud < -180 || longitud > 180)
            {
                return false;
            }

            resultado =
                $"{latitud.ToString("G29", CultureInfo.InvariantCulture)}, " +
                $"{longitud.ToString("G29", CultureInfo.InvariantCulture)}";

            return true;
        }
    }
}