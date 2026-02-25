using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.LogicaDeNegocio.Services;
using ONTHERUSH.UI.Helpers;
using ONTHERUSH.UI.Models;

namespace ONTHERUSH.UI.Controllers
{
    public class ReporteController : Controller
    {
        // GET: ReporteController

        private readonly ReporteViajesService _service;

        public ReporteController(ReporteViajesService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Viajes(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var vm = new ReporteViajesVM
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                vm.Viajes = _service.ObtenerReporte(fechaInicio.Value, fechaFin.Value);
            }

            return View(vm);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ExportarPdf(DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = _service.ObtenerReporte(fechaInicio, fechaFin);
            var pdf = PdfHelper.GenerarReporteViajes(datos);

            return File(pdf, "application/pdf", "ReporteViajes.pdf");
        }
    }
}
