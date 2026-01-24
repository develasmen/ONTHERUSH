using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;
using Microsoft.EntityFrameworkCore;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult PanelAdministrador()
        {
            return View();
        }

        public async Task<IActionResult> GestionarUsuarios()
        {
            var todosLosUsuarios = await _userManager.Users.ToListAsync();
            var usuariosSinRol = new List<ApplicationUser>();
            
            foreach (var user in todosLosUsuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    usuariosSinRol.Add(user);
                }
            }

            return View(usuariosSinRol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRol(string userId, string rol)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(rol))
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction("GestionarUsuarios");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("GestionarUsuarios");
            }

            if (!await _roleManager.RoleExistsAsync(rol))
            {
                TempData["Error"] = "Rol no válido.";
                return RedirectToAction("GestionarUsuarios");
            }

            var result = await _userManager.AddToRoleAsync(user, rol);

            if (result.Succeeded)
            {
                if (rol == "Conductor")
                {
                    var conductor = new Conductor
                    {
                        UserId = user.Id,
                        Estado = true,
                        FechaContratacion = DateTime.Now
                    };
                    _context.Conductores.Add(conductor);
                    await _context.SaveChangesAsync();
                }
                else if (rol == "Administrador")
                {
                    var administrador = new Administrador
                    {
                        UserId = user.Id,
                        Cargo = "Administrador General",
                        FechaContratacion = DateTime.Now
                    };
                    _context.Administradores.Add(administrador);
                    await _context.SaveChangesAsync();
                }

                TempData["Exito"] = $"Rol '{rol}' asignado exitosamente a {user.Nombre} {user.Apellido}.";
            }
            else
            {
                TempData["Error"] = "Error al asignar el rol.";
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