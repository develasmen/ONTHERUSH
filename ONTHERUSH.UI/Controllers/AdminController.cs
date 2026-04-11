using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;
using ONTHERUSH.LogicaDeNegocio.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ONTHERUSH.Abstracciones.Interfaces.Services;

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
        private readonly IVehiculoService _vehiculoService;
        private readonly ISolicitudCambioRutaService _solicitudCambioRutaService;
        private readonly IIncidenteService _incidenteService;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RutaAsignacionService rutaAsignacionService,
            ISolicitudCambioService solicitudCambioService,
            IViajeService viajeService,
            IReservaService reservaService,
            IVehiculoService vehiculoService,
            ISolicitudCambioRutaService solicitudCambioRutaService,
            IIncidenteService incidenteService)
        {
            _adminService = adminService;
            _userManager = userManager;
            _rutaAsignacionService = rutaAsignacionService;
            _solicitudCambioService = solicitudCambioService;
            _viajeService = viajeService;
            _reservaService = reservaService;
            _vehiculoService = vehiculoService;
            _solicitudCambioRutaService = solicitudCambioRutaService;
            _incidenteService = incidenteService;
        }

        public IActionResult PanelAdministrador()
        {
            return View();
        }

        public async Task<IActionResult> GestionarUsuarios()
        {
            var usuariosPendientesObj = await _adminService.ObtenerUsuariosSinRol();
            var usuariosPendientes = usuariosPendientesObj.Cast<ApplicationUser>().ToList();

            var usuariosActivos = await _userManager.Users
                .OrderByDescending(u => u.FechaRegistro)
                .ToListAsync();

            var usuariosConRoles = new List<(ApplicationUser usuario, IList<string> roles)>();

            foreach (var usuario in usuariosActivos)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                if (roles.Any())
                {
                    usuariosConRoles.Add((usuario, roles));
                }
            }

            ViewBag.UsuariosPendientes = usuariosPendientes;
            ViewBag.UsuariosActivos = usuariosConRoles;

            return View();
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
            var vehiculosObj = await _vehiculoService.ObtenerVehiculos();
            
            var vehiculos = vehiculosObj
                .Where(v => v.Estado == "Activo")
                .ToList();

            ViewBag.Conductores = conductores;
            ViewBag.Reservas = reservas;
            ViewBag.Vehiculos = vehiculos;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRutas(string conductorId, int vehiculoId, int[] reservaIds, int[] ordenes)
        {
            if (string.IsNullOrWhiteSpace(conductorId))
            {
                TempData["Error"] = "Debe seleccionar un conductor.";
                return RedirectToAction(nameof(AsignarRutas));
            }

            if (vehiculoId <= 0)
            {
                TempData["Error"] = "Debe seleccionar un vehículo.";
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
                vehiculoId: vehiculoId,
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


        // GET: Admin/GestionarVehiculos
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GestionarVehiculos()
        {
            var vehiculos = await _vehiculoService.ObtenerVehiculos();
            return View(vehiculos);
        }

        // POST: Admin/AgregarVehiculo
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarVehiculo(VehiculoDto vehiculo)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos";
                return RedirectToAction(nameof(GestionarVehiculos));
            }

            var resultado = await _vehiculoService.AgregarVehiculo(vehiculo);

            if (resultado.Exito)
                TempData["Exito"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(GestionarVehiculos));
        }

        // POST: Admin/EditarVehiculo
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarVehiculo(VehiculoDto vehiculo)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos";
                return RedirectToAction(nameof(GestionarVehiculos));
            }

            var resultado = await _vehiculoService.ActualizarVehiculo(vehiculo);

            if (resultado.Exito)
                TempData["Exito"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(GestionarVehiculos));
        }

        // POST: Admin/EliminarVehiculo
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarVehiculo(int id)
        {
            var resultado = await _vehiculoService.EliminarVehiculo(id);

            if (resultado.Exito)
                TempData["Exito"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(GestionarVehiculos));
        }


        // POST: Admin/EliminarUsuarioActivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuarioActivo(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "ID de usuario inválido";
                return RedirectToAction(nameof(GestionarUsuarios));
            }

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado";
                return RedirectToAction(nameof(GestionarUsuarios));
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            if (!roles.Any())
            {
                TempData["Error"] = "Este usuario no tiene un rol asignado";
                return RedirectToAction(nameof(GestionarUsuarios));
            }

            // no se permite que un admin se elimine a si mismo
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == userId)
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta de administrador";
                return RedirectToAction(nameof(GestionarUsuarios));
            }

            try
            {
                // DESHABILITAR en lugar de eliminar
                usuario.Estado = false;
                usuario.LockoutEnabled = true;
                usuario.LockoutEnd = DateTimeOffset.MaxValue;

                var result = await _userManager.UpdateAsync(usuario);

                if (result.Succeeded)
                {
                    TempData["Exito"] = $"Usuario {usuario.Nombre} {usuario.Apellido} deshabilitado exitosamente";
                }
                else
                {
                    TempData["Error"] = $"Error al deshabilitar usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al deshabilitar el usuario: {ex.Message}";
            }

            return RedirectToAction(nameof(GestionarUsuarios));
        }

        [HttpGet]
        public async Task<IActionResult> GestionarSolicitudesRuta()
        {
            var solicitudes = await _solicitudCambioRutaService.ObtenerSolicitudesPendientes();
            return View(solicitudes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarSolicitudRuta(int solicitudId)
        {
            await _solicitudCambioRutaService.AprobarSolicitud(solicitudId);

            TempData["Exito"] = "Solicitud de cambio de ruta aprobada correctamente.";
            return RedirectToAction(nameof(GestionarSolicitudesRuta));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarSolicitudRuta(int solicitudId, string comentarioAdministrador)
        {
            await _solicitudCambioRutaService.RechazarSolicitud(solicitudId, comentarioAdministrador);

            TempData["Exito"] = "Solicitud de cambio de ruta rechazada correctamente.";
            return RedirectToAction(nameof(GestionarSolicitudesRuta));
        }

        [HttpGet]
        public async Task<IActionResult> GestionarIncidentes()
        {
            var incidentes = await _incidenteService.ObtenerTodosLosIncidentesAsync();
            return View(incidentes);
        }


        [HttpGet]
        public IActionResult AgregarVehiculoForm()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditarVehiculoForm(int id)
        {
            var vehiculos = await _vehiculoService.ObtenerVehiculos();
            var vehiculoObj = vehiculos.FirstOrDefault(v => v.VehiculoId == id);
            
            if (vehiculoObj == null)
            {
                TempData["Error"] = "Vehículo no encontrado.";
                return RedirectToAction("GestionarVehiculos");
            }

            var vehiculo = vehiculoObj as VehiculoDto;
            return View(vehiculo);
        }
    }
}