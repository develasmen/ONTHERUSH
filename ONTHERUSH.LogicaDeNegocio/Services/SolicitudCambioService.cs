using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;
using Microsoft.AspNetCore.Identity;

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
            // Validaciones
            if (string.IsNullOrWhiteSpace(nuevoEmail))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "El nuevo email es requerido"
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

            // Validams que sean diferentes los correos
            if (usuario.Email == nuevoEmail)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "El nuevo email es igual al actual"
                };
            }

            // Creamos la solicitud
            var solicitud = new SolicitudCambio
            {
                UsuarioId = userId,
                TipoCambio = "Email",
                ValorActual = usuario.Email ?? "",
                ValorNuevo = nuevoEmail,
                Estado = "Pendiente",
                FechaSolicitud = DateTime.Now
            };

            return await _solicitudRepository.CrearSolicitud(solicitud);
        }

        public async Task<ResultadoOperacion> SolicitarCambioDireccion(string userId, string nuevaDireccion)
        {
            // Validacion
            if (string.IsNullOrWhiteSpace(nuevaDireccion))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "La nueva dirección es requerida"
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

            // Crear solicitud
            var solicitud = new SolicitudCambio
            {
                UsuarioId = userId,
                TipoCambio = "Direccion",
                ValorActual = usuario.Direccion ?? "",
                ValorNuevo = nuevaDireccion,
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
    }
}