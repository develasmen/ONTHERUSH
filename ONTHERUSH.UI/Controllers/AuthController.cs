using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            if (string.IsNullOrWhiteSpace(cedula) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contrasena) ||
                string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                ViewBag.Error = "Por favor completa los campos requeridos.";
                return View();
            }

            if (contrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseña no es correcta.";
                return View();
            }

            // Registro de Usuario
            var user = new ApplicationUser
            {
                UserName = correo,
                Email = correo,
                Nombre = nombre,
                Apellido = apellido,
                Cedula = cedula,
                Direccion = ubicacion,
                EmailConfirmed = true,
                Estado = true
            };

            var result = await _userManager.CreateAsync(user, contrasena);

            if (result.Succeeded)
            {
                ViewBag.Exito = "Registro exitoso. Su cuenta está pendiente de aprobación por un administrador.";
                return View();
            }
            else
            {
                // log de errores del Identity
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewBag.Error = "Error al crear la cuenta.";
                return View();
            }
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
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Por favor ingrese su correo y contraseña.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(correo);

            if (user == null)
            {
                ViewBag.Error = "Credenciales incorrectas.";
                return View();
            }

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

        // acá empieza la parte de cambiar la contraseña
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarContrasena(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                ViewBag.Error = "Por favor ingrese su correo.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(correo);

            if (user == null)
            {
                ViewBag.Exito = "Si el correo existe, se generará un enlace para restablecer la contraseña.";
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenUrl = System.Net.WebUtility.UrlEncode(token);

            var link = Url.Action(
                "RestablecerContrasena",
                "Auth",
                new { email = correo, token = tokenUrl },
                protocol: Request.Scheme
            );

            ViewBag.Exito = $"Enlace generado. Copie y pegue en el navegador: {link}";
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
        public async Task<IActionResult> RestablecerContrasena(string email, string token, string nuevaContrasena, string confirmarContrasena)
        {
            if (string.IsNullOrWhiteSpace(nuevaContrasena) || string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                ViewBag.Error = "Debe completar ambos campos.";
                ViewBag.Email = email;
                ViewBag.Token = token;
                return View();
            }

            if (nuevaContrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                ViewBag.Email = email;
                ViewBag.Token = token;
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Exito = "Contraseña actualizada (si el usuario existe).";
                return View();
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, nuevaContrasena);

            if (result.Succeeded)
            {
                ViewBag.Exito = "Contraseña actualizada correctamente. Ya puede iniciar sesión.";
                return View();
            }

            ViewBag.Error = string.Join(" | ", result.Errors.Select(e => e.Description));
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }



        // GET: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}