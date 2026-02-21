using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            IAuthService authService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(
            string cedula,
            string nombre,
            string apellido,
            string correo,
            string ubicacion,
            string contrasena,
            string confirmarContrasena)
        {
            var dto = new RegistroUsuarioDto
            {
                Cedula = cedula,
                Nombre = nombre,
                Apellido = apellido,
                Correo = correo,
                Ubicacion = ubicacion,
                Contrasena = contrasena,
                ConfirmarContrasena = confirmarContrasena
            };

            var resultado = await _authService.RegistrarUsuario(dto);

            if (resultado.Exito)
            {
                ViewBag.Exito = resultado.Mensaje;
            }
            else
            {
                ViewBag.Error = resultado.Mensaje;
            }

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            var dto = new LoginDto
            {
                Correo = correo,
                Contrasena = contrasena
            };

            var resultado = await _authService.IniciarSesion(dto);

            if (!resultado.Exito)
            {
                ViewBag.Error = resultado.Mensaje;
                return View();
            }

            var usuario = await _userManager.FindByEmailAsync(correo);
            var roles = await _userManager.GetRolesAsync(usuario);

            if (roles.Contains("Administrador"))
            {
                return RedirectToAction("PanelAdministrador", "Admin");
            }
            else if (roles.Contains("Conductor"))
            {
                return RedirectToAction("PanelConductor", "Conductor");
            }
            else if (roles.Contains("Pasajero"))
            {
                return RedirectToAction("PanelPasajero", "Usuario"); 
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarContrasena(string correo)
        {
            var resultado = await _authService.RecuperarContrasena(correo);

            if (resultado.Exito)
            {
                ViewBag.Exito = resultado.Mensaje;
            }
            else
            {
                ViewBag.Error = resultado.Mensaje;
            }

            return View();
        }

        [HttpGet]
        public IActionResult RestablecerContrasena(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                ViewBag.Error = "Enlace inválido.";
                return View();
            }

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerContrasena(
            string email,
            string token,
            string nuevaContrasena,
            string confirmarContrasena)
        {
            var resultado = await _authService.RestablecerContrasena(email, token, nuevaContrasena, confirmarContrasena);

            if (resultado.Exito)
            {
                ViewBag.Exito = resultado.Mensaje;
            }
            else
            {
                ViewBag.Error = resultado.Mensaje;
                ViewBag.Email = email;
                ViewBag.Token = token;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}