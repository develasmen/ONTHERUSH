using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Pasajero")]
    public class UsuarioController : Controller
    {
        public IActionResult PanelUsuario()
        {
            return View();
        }

        public IActionResult SolicitudEdicion()
        {
            return View();
        }

        public IActionResult ReservaTransporte()
        {
            return View();
        }
    }
}