using Microsoft.AspNetCore.Mvc;

namespace ONTHERUSH.UI.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult SolicitudEdicion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SolicitudEdicion(
            string nombre,
            string correo,
            string ubicacion,
            bool huboCambios,
            string detalleCambios)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(ubicacion))
            {
                ViewBag.Error = "Por favor llenar todos los campos necesarios.";
                return View();
            }

            if (!huboCambios)
            {
                ViewBag.Info = "No se realizaran cambios";
                return View();
            }

            ViewBag.Exito = "Su solicitud de actualización ha sido enviada a los administradores.";


            return View();
        }

        public IActionResult ReservaTransporte()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReservaTransporte(
            string nombre,
            string cedula,
            string correo,
            string telefono,
            string origen,
            string destino,
            string fecha,
            string horario,
            int cantidadPasajeros)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(cedula) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(telefono) ||
                string.IsNullOrWhiteSpace(origen) ||
                string.IsNullOrWhiteSpace(destino) ||
                string.IsNullOrWhiteSpace(fecha) ||
                string.IsNullOrWhiteSpace(horario) ||
                cantidadPasajeros <= 0)
            {
                ViewBag.Error = "Necesitar llenar todos los datos personales para realizar la reserva";
                return View();
            }
            if (cantidadPasajeros > 40)
            {
                ViewBag.Error = "Ha ocurrido un error";
                return View();
            }

            ViewBag.Exito = "Reserva Exitosa";


            return View();
        }
    }
}