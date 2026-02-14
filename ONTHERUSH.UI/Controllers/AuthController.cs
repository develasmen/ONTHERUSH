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
<<<<<<< HEAD
        private readonly RoleManager<IdentityRole> _roleManager;
=======
        private readonly UserManager<ApplicationUser> _userManager;
>>>>>>> Daniel

        public AuthController(
            IAuthService authService,
            SignInManager<ApplicationUser> signInManager,
<<<<<<< HEAD
            RoleManager<IdentityRole> roleManager)
=======
            UserManager<ApplicationUser> userManager)
>>>>>>> Daniel
        {
            _authService = authService;
            _signInManager = signInManager;
<<<<<<< HEAD
            _roleManager = roleManager;
=======
            _userManager = userManager;
>>>>>>> Daniel
        }

        // GET: Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: Registro
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

<<<<<<< HEAD
            if (result.Succeeded)
                {
                    ViewBag.Exito = "Registro exitoso. Su cuenta está pendiente de aprobación por un administrador.";
                    return View();
                }
=======
            if (resultado.Exito)
            {
                ViewBag.Exito = resultado.Mensaje;
            }
>>>>>>> Daniel
            else
            {
                ViewBag.Error = resultado.Mensaje;
            }

            return View();
        }











        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
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

<<<<<<< HEAD
            var user = await _userManager.FindByEmailAsync(correo);
            
            if (user == null)
=======
            // Obtener roles del usuario
            var usuario = await _userManager.FindByEmailAsync(correo);
            var roles = await _userManager.GetRolesAsync(usuario);

            // Redirigir según rol
            if (roles.Contains("Administrador"))
>>>>>>> Daniel
            {
                return RedirectToAction("PanelAdministrador", "Admin");
            }
            else if (roles.Contains("Conductor"))
            {
                return RedirectToAction("PanelConductor", "Conductor");
            }
            else if (roles.Contains("Pasajero"))
            {
                return RedirectToAction("ReservaTransporte", "Usuario");
            }

<<<<<<< HEAD
            // Verificar aprobacion de Administrador
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
            {
                ViewBag.Error = "Su cuenta está pendiente de aprobación por un administrador.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, contrasena, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
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
                    return RedirectToAction("ReservaTransporte", "Usuario");
                }
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Credenciales incorrectas.";
                return View();
            }
        }

        // GET: Logout
=======
            return RedirectToAction("Index", "Home");
        }

        // GET: RecuperarContrasena
        [HttpGet]
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        // POST: RecuperarContrasena
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

        // GET: RestablecerContrasena
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

        // POST: RestablecerContrasena
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

        // POST: Logout
>>>>>>> Daniel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}