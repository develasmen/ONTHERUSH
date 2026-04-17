using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.Abstracciones.Interfaces.Services;
using ONTHERUSH.AccesoADatos.Models;
using ONTHERUSH.LogicaDeNegocio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Conductor")]
    public class ConductorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RutaAsignacionService _rutaAsignacionService;
        private readonly IAdminService _adminService;
        private readonly IReservaService _reservaService;
        private readonly IViajeService _viajeService;
        private readonly IIncidenteService _incidenteService;
        private readonly ISolicitudCambioRutaService _solicitudCambioRutaService;

        public ConductorController(
            UserManager<ApplicationUser> userManager,
            RutaAsignacionService rutaAsignacionService,
            IAdminService adminService,
            IReservaService reservaService,
            IViajeService viajeService,
            IIncidenteService incidenteService,
            ISolicitudCambioRutaService solicitudCambioRutaService)
        {
            _userManager = userManager;
            _rutaAsignacionService = rutaAsignacionService;
            _adminService = adminService;
            _reservaService = reservaService;
            _viajeService = viajeService;
            _incidenteService = incidenteService;
            _solicitudCambioRutaService = solicitudCambioRutaService;
        }

        [HttpGet]
        public IActionResult PanelConductor()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ActualizarEstadoViaje()
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = "No se encontró el registro del conductor.";
                return RedirectToAction("PanelConductor");
            }

            var reservasObj = await _reservaService.ObtenerReservasAsignadasPorConductor(conductor.ConductorId);
            var reservas = reservasObj.Cast<Reserva>().ToList();

            if (reservas.Count == 0)
            {
                TempData["Mensaje"] = "No tiene un viaje activo para actualizar.";
                return RedirectToAction("PanelConductor");
            }

            var viajeIdActual = reservas.FirstOrDefault()?.ViajeId;

            if (viajeIdActual == null)
            {
                TempData["Mensaje"] = "No se encontró el viaje actual.";
                return RedirectToAction("PanelConductor");
            }

            var viajeObj = await _viajeService.ObtenerViajePorId(viajeIdActual.Value);
            var viajeActual = viajeObj as Viaje;

            if (viajeActual == null)
            {
                TempData["Mensaje"] = "No se encontró el viaje actual.";
                return RedirectToAction("PanelConductor");
            }

            var model = new ActualizarEstadoViajeViewModel
            {
                ViajeId = viajeActual.ViajeId,
                Ruta = string.IsNullOrWhiteSpace(viajeActual.NombreRuta) ? "Ruta actual" : viajeActual.NombreRuta,
                Salida = viajeActual.FechaHoraSalida,
                EstadoActual = string.IsNullOrWhiteSpace(viajeActual.Estado) ? "Programado" : viajeActual.Estado,
                Pasajeros = viajeActual.CantidadPasajeros,
                NuevoEstado = string.IsNullOrWhiteSpace(viajeActual.Estado) ? "Programado" : viajeActual.Estado
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstadoViaje(ActualizarEstadoViajeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = "No se encontró el registro del conductor.";
                return RedirectToAction("PanelConductor");
            }

            var resultado = await _viajeService.ActualizarEstadoViaje(model.ViajeId, model.NuevoEstado);

            TempData["Mensaje"] = resultado.Exito
                ? $"Estado del viaje actualizado a: {model.NuevoEstado}"
                : resultado.Mensaje;

            return RedirectToAction("VerViaje");
        }
        [HttpGet]
        public async Task<IActionResult> SolicitarActualizacionRuta()
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = "No se encontró el registro del conductor.";
                return RedirectToAction("PanelConductor");
            }

            var reservasObj = await _reservaService.ObtenerReservasAsignadasPorConductor(conductor.ConductorId);
            var reservas = reservasObj.Cast<Reserva>().ToList();

            if (reservas.Count == 0)
            {
                TempData["Mensaje"] = "No tiene reservas asignadas para solicitar cambios.";
                return RedirectToAction("PanelConductor");
            }

            var viajeId = reservas.First().ViajeId ?? 0;

            var paradas = new List<ParadaAsignadaDto>();
            int orden = 1;

            foreach (var r in reservas.OrderBy(r => r.FechaReserva))
            {
                paradas.Add(new ParadaAsignadaDto
                {
                    Id = r.ReservaId,
                    Orden = orden++,
                    NombreCliente = r.Usuario != null
                    ? $"{r.Usuario.Nombre} {r.Usuario.Apellido}"
                    : $"Usuario {r.UserId}",
                    Direccion = $"{r.Provincia}, {r.Canton}, {r.Distrito}",
                    Coordenadas = r.Usuario?.Direccion ?? "",
                    Estado = r.Estado
                });
            }

            var recogidos = paradas.Count(p => p.Estado == "Recogido");
            var pendientes = paradas.Count(p => p.Estado != "Recogido");

            var model = new SolicitudCambioRutaViewModel
            {
                ViajeId = viajeId,
                Ruta = $"Ruta {DateTime.Now:ddMMyyyy-HHmm}",
                Salida = reservas.First().FechaReserva,
                Estado = "Programado",
                Pasajeros = reservas.Count,
                TotalParadas = paradas.Count,
                Pendientes = pendientes,
                Recogidos = recogidos,
                DestinoFinal = "Aeropuerto Internacional Juan Santamaría",
                Paradas = paradas
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarActualizacionRuta(SolicitudCambioRutaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var conductorUserInvalid = await _userManager.GetUserAsync(User);
                if (conductorUserInvalid != null)
                {
                    var conductorObjInvalid = await _adminService.ObtenerConductorPorUserId(conductorUserInvalid.Id);
                    var conductorInvalid = conductorObjInvalid as Conductor;

                    if (conductorInvalid != null)
                    {
                        var reservasObj = await _reservaService.ObtenerReservasAsignadasPorConductor(conductorInvalid.ConductorId);
                        var reservas = reservasObj.Cast<Reserva>().ToList();

                        var paradas = new List<ParadaAsignadaDto>();
                        int orden = 1;

                        foreach (var r in reservas.OrderBy(r => r.FechaReserva))
                        {
                            paradas.Add(new ParadaAsignadaDto
                            {
                                Id = r.ReservaId,
                                Orden = orden++,
                                NombreCliente = r.Usuario != null
                                ? $"{r.Usuario.Nombre} {r.Usuario.Apellido}"
                                : $"Usuario {r.UserId}",
                                Direccion = $"{r.Provincia}, {r.Canton}, {r.Distrito}",
                                Coordenadas = r.Usuario?.Direccion ?? "",
                                Estado = r.Estado
                            });
                        }

                        model.Paradas = paradas;
                        model.TotalParadas = paradas.Count;
                        model.Pendientes = paradas.Count(p => p.Estado != "Recogido");
                        model.Recogidos = paradas.Count(p => p.Estado == "Recogido");
                        model.Pasajeros = reservas.Count;
                        model.DestinoFinal = "Aeropuerto Internacional Juan Santamaría";

                        var primeraReserva = reservas.FirstOrDefault();
                        if (primeraReserva != null)
                        {
                            model.Salida = primeraReserva.FechaReserva;
                        }
                    }
                }

                return View(model);
            }

            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = "No se encontró el registro del conductor.";
                return RedirectToAction("PanelConductor");
            }

            var dto = new SolicitudCambioRutaDTO
            {
                ViajeId = model.ViajeId,
                ConductorId = conductor.ConductorId,
                RutaActual = model.Ruta,
                NuevoOrdenPropuesto = model.DescripcionCambio,
                Motivo = model.Motivo
            };

            await _solicitudCambioRutaService.CrearSolicitud(dto);

            TempData["Mensaje"] = "Solicitud de cambio de ruta enviada correctamente al administrador.";
            return RedirectToAction("PanelConductor");
        }

        [HttpGet]
        public async Task<IActionResult> VerViaje()
        {
            return await VerPasajeros();
        }

        [HttpGet]
        public async Task<IActionResult> VerPasajeros()
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = " No se encontró el registro del conductor.";
                return View("VerViaje", new List<ParadaAsignadaDto>());
            }

            // Obtener TODAS las reservas del conductor
            var reservasObj = await _reservaService.ObtenerReservasAsignadasPorConductor(conductor.ConductorId);
            var todasLasReservas = reservasObj.Cast<Reserva>().ToList();

            if (todasLasReservas.Count == 0)
            {
                TempData["Mensaje"] = " No tienes reservas asignadas todavía.";
                ViewBag.EstadoViaje = "Programado";
                ViewBag.RutaViaje = "Ruta actual";
                ViewBag.Vehiculo = "No asignado";
                return View("VerViaje", new List<ParadaAsignadaDto>());
            }

            // Agrupar reservas por ViajeId
            var reservasPorViaje = todasLasReservas
                .Where(r => r.ViajeId != null)
                .GroupBy(r => r.ViajeId.Value)
                .ToList();

            if (reservasPorViaje.Count == 0)
            {
                TempData["Mensaje"] = " No tienes viajes asignados.";
                ViewBag.EstadoViaje = "Programado";
                ViewBag.RutaViaje = "Ruta actual";
                ViewBag.Vehiculo = "No asignado";
                return View("VerViaje", new List<ParadaAsignadaDto>());
            }

            // Buscar el primer viaje NO finalizado
            Viaje? viajeActivo = null;
            foreach (var grupo in reservasPorViaje)
            {
                var viajeObj = await _viajeService.ObtenerViajePorId(grupo.Key);
                var viaje = viajeObj as Viaje;

                if (viaje != null && viaje.Estado != "Finalizado" && viaje.Estado != "Completado")
                {
                    viajeActivo = viaje;
                    break;
                }
            }

            // Si no hay viaje activo, mostrar mensaje
            if (viajeActivo == null)
            {
                TempData["Mensaje"] = " No tienes un viaje activo. Todos tus viajes están finalizados.";
                ViewBag.EstadoViaje = "Sin viajes activos";
                ViewBag.RutaViaje = "N/A";
                ViewBag.Vehiculo = "N/A";
                return View("VerViaje", new List<ParadaAsignadaDto>());
            }

            // Configurar ViewBag con datos del viaje activo
            ViewBag.EstadoViaje = viajeActivo.Estado;
            ViewBag.RutaViaje = viajeActivo.NombreRuta;

            if (viajeActivo.Vehiculo != null)
            {
                ViewBag.Vehiculo = $"{viajeActivo.Vehiculo.Placa} - {viajeActivo.Vehiculo.Marca} {viajeActivo.Vehiculo.Modelo}";
            }
            else
            {
                ViewBag.Vehiculo = "No asignado";
            }

            // Filtrar SOLO las reservas del viaje activo
            var reservasDelViajeActivo = todasLasReservas
                .Where(r => r.ViajeId == viajeActivo.ViajeId)
                .ToList();

            // Crear paradas
            var paradas = new List<ParadaAsignadaDto>();
            int orden = 1;

            foreach (var r in reservasDelViajeActivo)
            {
                paradas.Add(new ParadaAsignadaDto
                {
                    Id = r.ReservaId,
                    Orden = orden++,
                    NombreCliente = r.Usuario != null
                    ? $"{r.Usuario.Nombre} {r.Usuario.Apellido}"
                    : $"Usuario {r.UserId}",
                    Direccion = $"{r.Provincia}, {r.Canton}, {r.Distrito}",
                    Coordenadas = r.Usuario?.Direccion ?? "",
                    Estado = r.Estado
                });
            }

            return View("VerViaje", paradas.OrderBy(x => x.Orden).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarRecogido(int id)
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var reservaObj = await _reservaService.ObtenerReservaPorId(id);
            var reserva = reservaObj as Reserva;

            if (reserva == null)
            {
                TempData["Mensaje"] = " No se encontró la reserva.";
                return RedirectToAction("VerViaje");
            }

            reserva.Estado = "Recogido";
            var resultado = await _reservaService.ActualizarReserva(reserva);

            TempData["Mensaje"] = resultado.Exito
                ? " Reserva marcada como recogida."
                : $" {resultado.Mensaje}";

            return RedirectToAction("VerViaje");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarViaje()
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = " Conductor no encontrado.";
                return RedirectToAction("VerViaje");
            }

            var reservasObj = await _reservaService.ObtenerReservasAsignadasPorConductor(conductor.ConductorId);
            var reservas = reservasObj.Cast<Reserva>().ToList();

            if (reservas.Count == 0)
            {
                TempData["Mensaje"] = " No hay viaje activo.";
                return RedirectToAction("VerViaje");
            }

            var viajeId = reservas.First().ViajeId;

            if (viajeId == null)
            {
                TempData["Mensaje"] = " No se encontró el viaje.";
                return RedirectToAction("VerViaje");
            }

            var resultado = await _viajeService.FinalizarViaje(viajeId.Value);

            TempData["Mensaje"] = resultado.Exito
                ? " Viaje finalizado correctamente."
                : $" {resultado.Mensaje}";

            return RedirectToAction("VerViaje");
        }

        [HttpGet]
        public IActionResult ReportarIncidente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportarIncidente(CrearIncidenteDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = "No se encontró el registro del conductor.";
                return RedirectToAction("PanelConductor");
            }

            await _incidenteService.CrearIncidenteAsync(conductorUser.Id, dto);

            TempData["Mensaje"] = "Incidente reportado correctamente.";
            return RedirectToAction("PanelConductor");
        }

        [HttpGet]
        public async Task<IActionResult> MisIncidentes()
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var incidentes = await _incidenteService.ObtenerIncidentesPorConductorAsync(conductorUser.Id);
            return View(incidentes);
        }

        [HttpGet]
        public async Task<IActionResult> HistorialViajes()
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = "No se encontró el registro del conductor.";
                return View(new List<ReporteViajeDTO>());
            }

            var historial = await _viajeService.ObtenerHistorialViajesPorConductor(conductor.ConductorId);
            return View(historial);
        }

        [HttpGet]
        public async Task<IActionResult> DetalleViaje(int id)
        {
            var conductorUser = await _userManager.GetUserAsync(User);
            if (conductorUser == null)
                return RedirectToAction("Login", "Auth");

            var conductorObj = await _adminService.ObtenerConductorPorUserId(conductorUser.Id);
            var conductor = conductorObj as Conductor;

            if (conductor == null)
            {
                TempData["Mensaje"] = "No se encontró el registro del conductor.";
                return RedirectToAction("HistorialViajes");
            }

            var viaje = await _viajeService.ObtenerDetalleViajePorConductor(id, conductor.ConductorId);

            if (viaje == null)
            {
                TempData["Mensaje"] = "No se encontró el viaje solicitado.";
                return RedirectToAction("HistorialViajes");
            }

            return View(viaje);
        }
    }
}