using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;
using System.Linq;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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
            {
                TempData["Exito"] = resultado.Mensaje;
            }
            else
            {
                TempData["Error"] = resultado.Mensaje;
            }

            return RedirectToAction("GestionarUsuarios");
        }

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

        public IActionResult AsignarRutas()
        {
            return View();
        }
    }
}