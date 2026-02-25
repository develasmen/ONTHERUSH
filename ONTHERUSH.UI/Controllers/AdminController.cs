using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;
using ONTHERUSH.LogicaDeNegocio.Services;
using System.Linq;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RutaAsignacionService _rutaAsignacionService;
        private readonly ISolicitudCambioService _solicitudCambioService;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RutaAsignacionService rutaAsignacionService,
            ISolicitudCambioService solicitudCambioService)
        {
            _adminService = adminService;
            _userManager = userManager;
            _rutaAsignacionService = rutaAsignacionService;
            _solicitudCambioService = solicitudCambioService;
        }

        public IActionResult PanelAdministrador()
        {
            return View();
        }

        public async Task<IActionResult> GestionarUsuarios()
        {
            var usuariosSinRolObj = await _adminService.ObtenerUsuariosSinRol();

            var usuariosSinRol = usuariosSinRolObj
                .Cast<ApplicationUser>()
                .ToList();

            return View(usuariosSinRol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRol(string userId, string rol)
        {
            var resultado = await _adminService.AsignarRol(userId, rol);

            if (resultado.Exito)
                TempData["Exito"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction("GestionarUsuarios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarUsuario(string userId)
        {
            var resultado = await _adminService.RechazarUsuario(userId);

            if (resultado.Exito)
            {
                TempData["Exito"] = resultado.Mensaje;
            }
            else
            {
                TempData["Error"] = resultado.Mensaje;
            }

            return RedirectToAction("GestionarUsuarios");
        }

        // GESTIONAR SOLICITUDES DE CAMBIO

        public async Task<IActionResult> GestionarSolicitudes()
        {
            var solicitudesObj = await _solicitudCambioService.ObtenerSolicitudesPendientes();

            var solicitudes = solicitudesObj
                .Cast<SolicitudCambio>()
                .ToList();

            return View(solicitudes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarSolicitud(int solicitudId)
        {
            var resultado = await _solicitudCambioService.AprobarSolicitud(solicitudId);

            if (resultado.Exito)
            {
                TempData["Exito"] = resultado.Mensaje;
            }
            else
            {
                TempData["Error"] = resultado.Mensaje;
            }

            return RedirectToAction("GestionarSolicitudes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarSolicitudCambio(int solicitudId, string motivoRechazo)
        {
            var resultado = await _solicitudCambioService.RechazarSolicitud(solicitudId, motivoRechazo);

            if (resultado.Exito)
            {
                TempData["Exito"] = resultado.Mensaje;
            }
            else
            {
                TempData["Error"] = resultado.Mensaje;
            }

            return RedirectToAction("GestionarSolicitudes");
        }

        // ASIGNAR RUTAS

        [HttpGet]
        public async Task<IActionResult> AsignarRutas()
        {
            var conductores = await _userManager.GetUsersInRoleAsync("Conductor");
            var pasajeros = await _userManager.GetUsersInRoleAsync("Pasajero");

            ViewBag.Conductores = conductores;
            ViewBag.Pasajeros = pasajeros;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRutas(string conductorId, string[] pasajeroIds, int[] ordenes)
        {
            if (string.IsNullOrWhiteSpace(conductorId))
            {
                TempData["Error"] = "Debe seleccionar un conductor.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            if (pasajeroIds == null || pasajeroIds.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar al menos un pasajero.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            if (ordenes == null || ordenes.Length == 0)
            {
                TempData["Error"] = "Debe indicar el orden de la ruta.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            if (pasajeroIds.Length != ordenes.Length)
            {
                TempData["Error"] = "El orden no coincide con los pasajeros seleccionados. Revise los datos.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            var paradas = new List<ParadaAsignadaDto>();

            for (int i = 0; i < pasajeroIds.Length; i++)
            {
                var pasajero = await _userManager.FindByIdAsync(pasajeroIds[i]);
                if (pasajero == null) continue;

                paradas.Add(new ParadaAsignadaDto
                {
                    Id = i + 1,
                    Orden = ordenes[i],
                    NombreCliente = $"{pasajero.Nombre} {pasajero.Apellido}",
                    Direccion = pasajero.Direccion ?? "",
                    Estado = "Pendiente"
                });
            }

            _rutaAsignacionService.AsignarRuta(conductorId, paradas);

            TempData["Exito"] = "Ruta asignada correctamente al conductor.";
            return RedirectToAction(nameof(AsignarRutas));
        }

        // OTRAS VISTAS

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult DescargarReportes()
        {
            return View();
        }

        public IActionResult ActualizarSolicitud()
        {
            return View();
        }
    }
}