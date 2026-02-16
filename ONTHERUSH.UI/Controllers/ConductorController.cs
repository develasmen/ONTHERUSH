using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.DTOs;
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

        public ConductorController(
            UserManager<ApplicationUser> userManager,
            RutaAsignacionService rutaAsignacionService)
        {
            _userManager = userManager;
            _rutaAsignacionService = rutaAsignacionService;
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
        public async Task<IActionResult> VerPasajeros()
        {
            var conductor = await _userManager.GetUserAsync(User);
            if (conductor == null)
                return RedirectToAction("Login", "Auth");

            // Trae la ruta asignada por el Admin (en memoria)
            var paradas = _rutaAsignacionService.ObtenerRuta(conductor.Id);

            // Si no hay ruta asignada, mandamos lista vacía con mensajito
            if (paradas == null || paradas.Count == 0)
            {
                TempData["Mensaje"] = "⚠️ No tienes una ruta asignada todavía.";
                return View(new List<ParadaAsignadaDto>());
            }

            // Recogidos por conductor (TempData separado por conductor)
            var key = $"Recogidos_{conductor.Id}";
            var recogidos = TempData[key] as string;
            var idsRecogidos = new HashSet<int>();

            if (!string.IsNullOrWhiteSpace(recogidos))
            {
                idsRecogidos = recogidos.Split(',')
                    .Select(x => int.TryParse(x, out var id) ? id : 0)
                    .Where(id => id > 0)
                    .ToHashSet();
            }

            foreach (var p in paradas)
            {
                if (idsRecogidos.Contains(p.Id))
                    p.Estado = "Recogido";
            }

            TempData.Keep(key);

            return View(paradas.OrderBy(x => x.Orden).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarRecogido(int id)
        {
            var conductor = await _userManager.GetUserAsync(User);
            if (conductor == null)
                return RedirectToAction("Login", "Auth");

            var key = $"Recogidos_{conductor.Id}";
            var recogidos = TempData[key] as string;
            var set = new HashSet<int>();

            if (!string.IsNullOrWhiteSpace(recogidos))
            {
                set = recogidos.Split(',')
                    .Select(x => int.TryParse(x, out var n) ? n : 0)
                    .Where(n => n > 0)
                    .ToHashSet();
            }

            set.Add(id);

            TempData[key] = string.Join(",", set);
            TempData["Mensaje"] = "✅ Pasajero marcado como recogido.";
            TempData.Keep(key);

            return RedirectToAction("VerPasajeros");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarViaje()
        {
            var conductor = await _userManager.GetUserAsync(User);
            if (conductor == null)
                return RedirectToAction("Login", "Auth");

            // Borra ruta asignada
            _rutaAsignacionService.LimpiarRuta(conductor.Id);

            // Limpia recogidos del conductor
            var key = $"Recogidos_{conductor.Id}";
            TempData.Remove(key);

            TempData["Mensaje"] = "✅ Viaje finalizado. Ruta completada.";
            return RedirectToAction("VerPasajeros");
        }
    }
}