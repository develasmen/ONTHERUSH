using Microsoft.AspNetCore.Mvc;

namespace ONTHERUSH.UI.Controllers
{
    public class AdminController : Controller
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
                ViewBag.Error = "Por favor llene todos los campos.";
                return View();
            }

            var adminDemoCorreo = "admin@demo.com";
            var adminDemoPass = "admin123";

            if (correo == adminDemoCorreo && contrasena == adminDemoPass)
            {
                TempData["MensajeBienvenidaAdmin"] = "Inicio de sesión exitoso. Bienvenido al panel de administradores.";
                return RedirectToAction("PanelAdministrador");
            }
            else
            {
                ViewBag.Error = "Las credenciales estan incorrectas.";
                return View();
            }
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(
            string nombre,
            string correo,
            string contrasena,
            string confirmarContrasena)
        {

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contrasena) ||
                string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                ViewBag.Error = "Por favor llene todos los campos.";
                return View();
            }

            if (contrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            var adminDemoCorreo = "admin@demo.com";
            if (correo == adminDemoCorreo)
            {
                ViewBag.Error = "Las credenciales ya están registradas.";
                return View();
            }


            ViewBag.Exito = "Registro exitoso. El administrador ha sido registrado.";

            return View();
        }


        public IActionResult PanelAdministrador()
        {
            return View();
        }

        public IActionResult DescargarReportes()
        {
            return View();
        }


        [HttpPost]
        public IActionResult DescargarReportes(string idConductor, string estadoReporte)
        {


            if (string.IsNullOrWhiteSpace(idConductor))
            {
                ViewBag.Error = "Por favor ingrese un identificador de conductor.";
                return View();
            }

            if (estadoReporte == "error")
            {

                ViewBag.Error = "Hubo un error al descargar el reporte, porfavor volver a intentar.";
                return View();
            }
            else if (estadoReporte == "nodisponible")
            {
          
                ViewBag.Error = "El reporte seleccionado no se encuentra disponible.";
                return View();
            }
            else
            {
       
                ViewBag.Exito = "El reporte se ha descargado en formato xls (simulado).";
                return View();
            }
        }

        public IActionResult ActualizarSolicitud()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ActualizarSolicitud(
            string nombreUsuario,
            string nuevoCorreo,
            string nuevaUbicacion,
            bool datosValidos,
            bool autorizar)
        {
       
            if (!datosValidos)
            {
                ViewBag.Error = "Los datos son invalidos, por favor revisar.";
                return View();
            }

            if (autorizar)
            {
                ViewBag.Exito = "La informacion ha sido actualizada exitosamente.";
            }
            else
            {
                ViewBag.Info = "La autorizacion ha sido rechazada.";
            }

            return View();
        }

        public IActionResult AsignarRutas()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AsignarRutas(
            string idConductor,
            string ruta,
            bool conductorDisponible,
            bool esActualizacionRuta)
        {
            if (string.IsNullOrWhiteSpace(idConductor) || string.IsNullOrWhiteSpace(ruta))
            {
                ViewBag.Error = "Por favor complete todos los datos de la ruta y del conductor.";
                return View();
            }

            if (!conductorDisponible)
            {
                ViewBag.Error = "El conductor no esta disponible";
                return View();
            }

            if (esActualizacionRuta)
            {
                ViewBag.Info = "Sesión no activa";
                return View();
            }

            ViewBag.Exito = "La ruta ha sido asignada exitosamente al conductor.";

            return View();
        }

        public IActionResult CerrarSesion(string estado)
        {
            switch (estado)
            {
                case "activa":
                    TempData["MensajeSesionAdmin"] = "Sesión cerrada correctamente.";
                    break;

                case "inactiva":
                    TempData["MensajeSesionAdmin"] = "Sesión no activa";
                    break;

                case "sinSesion":
                default:
                    TempData["MensajeSesionAdmin"] = "No hay ninguna sesión activa.";
                    break;
            }

            return RedirectToAction("Login");
        }
    }
}
