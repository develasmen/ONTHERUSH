using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Conductor")]
    public class ConductorController : Controller
    {
        public IActionResult PanelConductor()
        {
            return View();
        }

        public IActionResult ActualizarEstadoViaje()
        {
            return View();
        }

        public IActionResult SolicitarActualizacionRuta()
        {
            return View();
        }

        public IActionResult VerPasajeros()
        {
            return View();
        }
    }
}