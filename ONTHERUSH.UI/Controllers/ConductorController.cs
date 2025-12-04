using Microsoft.AspNetCore.Mvc;

namespace ONTHERUSH.UI.Controllers
{
    public class ConductorController : Controller
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
                ViewBag.Error = "Por favor rellenar todos los campos necesarios.";
                return View();
            }

         
            var correoDemo = "conductor@demo.com";
            var passDemo = "cond123";

            if (correo == correoDemo && contrasena == passDemo)
            {
         
                TempData["MensajeBienvenidaConductor"] = "Inicio de sesión exitoso. Bienvenido al panel de conductores.";
                return RedirectToAction("PanelConductor");
            }
            else
            {
                ViewBag.Error = "Las credenciales son incorrectas, por favor intente de nuevo.";
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
            string numero,
            string correo,
            string direccion)
        {
            if (string.IsNullOrWhiteSpace(cedula) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(numero) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(direccion))
            {
                ViewBag.Error = "Los campos estan vacios o incorrectos, por favor revisar.";
                return View();
            }

            var correoDemo = "conductor@demo.com";
            if (correo == correoDemo)
            {
                ViewBag.Error = "Sus credenciales han sido registradas exitosamente.";
                return View();
            }

            ViewBag.Exito = "Informacion guardada con exito";

            return View();
        }
        public IActionResult PanelConductor()
        {
            return View();
        }
        public IActionResult ActualizarEstadoViaje()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ActualizarEstadoViaje(string idViaje, string accion)
        {

            if (string.IsNullOrWhiteSpace(idViaje))
            {
                ViewBag.Error = "Por favor ingrese el identificador del viaje.";
                return View();
            }

            if (accion == "error")
            {
                ViewBag.Error = "No es posible actualizar el viaje";
            }
            else if (accion == "yaActualizada")
            {
                ViewBag.Info = "La ruta ya ha sido actualizada";
            }
            else
            {
                ViewBag.Exito = "El estado del viaje se ha actualizado correctamente.";
            }

            return View();
        }

        public IActionResult SolicitarActualizacionRuta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SolicitarActualizacionRuta(
            string idRuta,
            string motivo,
            bool hayCambios)
        {
            if (string.IsNullOrWhiteSpace(idRuta) || string.IsNullOrWhiteSpace(motivo))
            {
                ViewBag.Error = "Ha ocurrido un error , intente nuevamente";
                return View();
            }

            if (!hayCambios)
            {
                ViewBag.Info = "No hay nigun cambio por realizar";
                return View();
            }

            ViewBag.Exito = "Su solicitud ha sido enviada exitosamente.";

            return View();
        }

        public IActionResult VerPasajeros()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerPasajeros(string idRuta, string estadoLista)
        {

            if (string.IsNullOrWhiteSpace(idRuta))
            {
                ViewBag.Error = "Por favor ingrese la ruta.";
                return View();
            }

            if (estadoLista == "error")
            {
                ViewBag.Error = "Ha ocurrido un error";
                return View();
            }
            else if (estadoLista == "vacia")
            {
                ViewBag.Info = "No hay pasajeros en su ruta";
                return View();
            }
            else
            {
                ViewBag.Exito = "Lista de pasajeros cargada correctamente (simulada).";

                ViewBag.Pasajeros = new[]
                {
                    "Juan Pérez",
                    "María López",
                    "Carlos Rodríguez"
                };

                return View();
            }
        }

        public IActionResult CerrarSesion(string estado)
        {
            switch (estado)
            {
                case "activa":
                    TempData["MensajeSesionConductor"] = "Sesión cerrada correctamente.";
                    break;

                case "inactiva":
                    TempData["MensajeSesionConductor"] = "Sesión no activa";
                    break;

                case "sinSesion":
                default:
                    TempData["MensajeSesionConductor"] = "No hay ninguna sesión activa.";
                    break;
            }

            return RedirectToAction("Login");
        }
    }
}
