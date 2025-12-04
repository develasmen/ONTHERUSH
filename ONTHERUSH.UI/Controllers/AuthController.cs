using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ONTHERUSH.UI.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Por favor llenar todos los campos requeridos.";
                return View();
            }

            var correoDemo = "usuario@demo.com";
            var contrasenaDemo = "1234";

            if (correo == correoDemo && contrasena == contrasenaDemo)
            {
                TempData["MensajeBienvenida"] = "Inicio de sesión exitoso. ¡Bienvenido al panel de usuario!";
                return RedirectToAction("PanelUsuario");
            }
            else
            {
                ViewBag.Error = "Las credenciales son incorrectas, por favor intentar de nuevo.";
                return View();
            }
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(
            string cedula,
            string nombre,
            string correo,
            string ubicacion,
            string contrasena,
            string confirmarContrasena)
        {
            if (string.IsNullOrWhiteSpace(cedula) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(ubicacion) ||
                string.IsNullOrWhiteSpace(contrasena) ||
                string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                ViewBag.Error = "Por favor llenar todos los campos requeridos.";
                return View();
            }

            if (contrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            ViewBag.Exito = "Registro exitoso. ¡Su cuenta ha sido creada!";

            return View();
        }
        public IActionResult OlvidoContrasena()
        {
            return View();
        }

        [HttpPost]
        public IActionResult OlvidoContrasena(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                ViewBag.Error = "Por favor ingresar un correo electrónico.";
                return View();
            }

            if (!EsCorreoValido(correo))
            {
                ViewBag.Error = "Formato de correo inválido.";
                return View();
            }

            var correoDemoRegistrado = "usuario@demo.com";

            if (!string.Equals(correo, correoDemoRegistrado, System.StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Correo no encontrado.";
                return View();
            }

            ViewBag.Exito = "Se ha enviado un correo con el enlace para restablecer su contraseña.";

            return View();
        }

        private bool EsCorreoValido(string correo)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(correo);
        }

      
        public IActionResult PanelUsuario()
        {
            return View();
        }

      
        public IActionResult CerrarSesionUsuario(string estado)
        {
            switch (estado)
            {
                case "activa":
                    TempData["MensajeSesion"] = "Sesión cerrada correctamente.";
                    break;

                case "expirada":
                    TempData["MensajeSesion"] = "Sesión expirada";
                    break;

                case "inactiva":
                default:
                    TempData["MensajeSesion"] = "Sesión no activa";
                    break;
            }

            return RedirectToAction("Login");
        }
    }
}
