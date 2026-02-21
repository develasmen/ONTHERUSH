using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.Interfaces;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Pasajero")]
    public class UsuarioController : Controller
    {
        private readonly IReservaService _reservaService;
        private readonly ISolicitudCambioService _solicitudCambioService;

        public UsuarioController(
            IReservaService reservaService,
            ISolicitudCambioService solicitudCambioService)
        {
            _reservaService = reservaService;
            _solicitudCambioService = solicitudCambioService;
        }

        // Panel Principal del Pasajero
        public IActionResult PanelPasajero()
        {
            return View();
        }

        // GET: Hacer Reserva
        public IActionResult ReservaTransporte()
        {
            return View();
        }

        // POST: Hacer Reserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservaTransporte(TimeSpan horaDestino, string provincia, string canton, string distrito)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var resultado = await _reservaService.CrearReserva(userId, horaDestino, provincia, canton, distrito);

            if (resultado.Exito)
            {
                TempData["Exito"] = resultado.Mensaje;
                return RedirectToAction("PanelPasajero");
            }
            else
            {
                ViewBag.Error = resultado.Mensaje;
                return View();
            }
        }

        // GET: Solicitar Cambios
        public IActionResult SolicitudEdicion()
        {
            return View();
        }

        // POST: Solicitar Cambio Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarCambioEmail(string nuevoEmail)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var resultado = await _solicitudCambioService.SolicitarCambioEmail(userId, nuevoEmail);

            if (resultado.Exito)
            {
                TempData["Exito"] = resultado.Mensaje;
                return RedirectToAction("PanelPasajero");
            }
            else
            {
                ViewBag.Error = resultado.Mensaje;
                return View("SolicitudEdicion");
            }
        }

        // POST: Solicitar Cambio Dirección
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarCambioDireccion(string nuevaDireccion)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var resultado = await _solicitudCambioService.SolicitarCambioDireccion(userId, nuevaDireccion);

            if (resultado.Exito)
            {
                TempData["Exito"] = resultado.Mensaje;
                return RedirectToAction("PanelPasajero");
            }
            else
            {
                ViewBag.Error = resultado.Mensaje;
                return View("SolicitudEdicion");
            }
        }
    }
}