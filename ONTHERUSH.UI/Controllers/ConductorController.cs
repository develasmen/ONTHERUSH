using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;
using ONTHERUSH.LogicaDeNegocio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public ConductorController(
            UserManager<ApplicationUser> userManager,
            RutaAsignacionService rutaAsignacionService,
            IAdminService adminService,
            IReservaService reservaService,
            IViajeService viajeService)
        {
            _userManager = userManager;
            _rutaAsignacionService = rutaAsignacionService;
            _adminService = adminService;
            _reservaService = reservaService;
            _viajeService = viajeService;
        }

        [HttpGet]
        public IActionResult PanelConductor()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ActualizarEstadoViaje()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SolicitarActualizacionRuta()
        {
            return View();
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

            var reservasObj = await _reservaService.ObtenerReservasAsignadasPorConductor(conductor.ConductorId);
            var reservas = reservasObj.Cast<Reserva>().ToList();

            if (reservas.Count == 0)
            {
                TempData["Mensaje"] = " No tienes reservas asignadas todavía.";
                return View("VerViaje", new List<ParadaAsignadaDto>());
            }

            var paradas = new List<ParadaAsignadaDto>();
            int orden = 1;

            foreach (var r in reservas)
            {
                paradas.Add(new ParadaAsignadaDto
                {
                    Id = r.ReservaId,
                    Orden = orden++,
                    NombreCliente = r.Usuario != null
                        ? $"{r.Usuario.Nombre} {r.Usuario.Apellido}"
                        : $"Usuario {r.UserId}",
                    Direccion = $"{r.Provincia}, {r.Canton}, {r.Distrito}",
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
    }
}