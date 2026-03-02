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
        private readonly IViajeService _viajeService;
        private readonly IReservaService _reservaService;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RutaAsignacionService rutaAsignacionService,
            ISolicitudCambioService solicitudCambioService,
            IViajeService viajeService,
            IReservaService reservaService)
        {
            _adminService = adminService;
            _userManager = userManager;
            _rutaAsignacionService = rutaAsignacionService;
            _solicitudCambioService = solicitudCambioService;
            _viajeService = viajeService;
            _reservaService = reservaService;
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

            var reservasObj = await _reservaService.ObtenerReservasPendientes();
            var reservas = reservasObj.Cast<Reserva>().ToList();

            ViewBag.Conductores = conductores;
            ViewBag.Reservas = reservas;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRutas(string conductorId, int[] reservaIds, int[] ordenes)
        {
            if (string.IsNullOrWhiteSpace(conductorId))
            {
                TempData["Error"] = "Debe seleccionar un conductor.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            if (reservaIds == null || reservaIds.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar al menos una reserva.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            if (ordenes == null || ordenes.Length == 0)
            {
                TempData["Error"] = "Debe indicar el orden de la ruta.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            if (reservaIds.Length != ordenes.Length)
            {
                TempData["Error"] = "El orden no coincide con las reservas seleccionadas. Revise los datos.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            // 1) Obtener el Conductor (tabla Conductores) a partir del userId (conductorId)
            var conductorUser = await _userManager.FindByIdAsync(conductorId);
            if (conductorUser == null)
            {
                TempData["Error"] = "Conductor no válido.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorId); // si no existe, lo vemos en el siguiente paso
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Error"] = "No se encontró el registro del conductor en la tabla Conductores.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            // 2) Obtener AdministradorId (tabla Administradores) desde el usuario logueado
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
            {
                TempData["Error"] = "No se pudo identificar el administrador.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            var adminObj = await _adminService.ObtenerAdministradorPorUserId(adminUser.Id); // si no existe, lo vemos en el siguiente paso
            var admin = adminObj as Administrador;

            if (admin == null)
            {
                TempData["Error"] = "No se encontró el registro del administrador en la tabla Administradores.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            // 3) Crear VIAJE en BD (VehiculoId dummy = 1)
            // NombreRuta / PuntoPartida / Paradas: por ahora simple (luego se mejora)
            var nombreRuta = $"Ruta {DateTime.Now:ddMMyyyy-HHmm}";
            var puntoPartida = "Punto de partida pendiente";

            // Paradas: armamos texto con provincia/canton/distrito en el orden indicado
            var pares = reservaIds.Zip(ordenes, (rid, ord) => new { rid, ord })
                                  .OrderBy(x => x.ord)
                                  .ToList();

            var paradasTexto = new List<string>();

            foreach (var item in pares)
            {
                var rObj = await _reservaService.ObtenerReservaPorId(item.rid);
                var r = rObj as Reserva;
                if (r == null) continue;

                paradasTexto.Add($"{item.ord}. {r.Provincia}, {r.Canton}, {r.Distrito}");
            }

            var resultadoViaje = await _viajeService.CrearViaje(
                conductorId: conductor.ConductorId,
                asignadoPorAdminId: admin.AdministradorId,
                vehiculoId: 1,
                nombreRuta: nombreRuta,
                puntoPartida: puntoPartida,
                paradas: string.Join(" | ", paradasTexto),
                fechaHoraSalida: DateTime.Now.AddMinutes(30),
                horaEstimadaLlegada: DateTime.Now.AddHours(2),
                cantidadPasajeros: reservaIds.Length,
                observaciones: "Asignado desde administrador"
            );

            if (!resultadoViaje.Exito)
            {
                TempData["Error"] = resultadoViaje.Mensaje;
                return RedirectToAction(nameof(AsignarRutas));
            }

            // El ViajeId quedó en Datos
            int viajeIdCreado = Convert.ToInt32(resultadoViaje.Datos);

            // 4) Actualizar reservas: ViajeId + Estado
            foreach (var item in pares)
            {
                var rObj = await _reservaService.ObtenerReservaPorId(item.rid);
                var r = rObj as Reserva;
                if (r == null) continue;

                r.ViajeId = viajeIdCreado;
                r.Estado = "Asignada";

                await _reservaService.ActualizarReserva(r);
            }

            TempData["Exito"] = $"Viaje #{viajeIdCreado} creado y reservas asignadas correctamente.";

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