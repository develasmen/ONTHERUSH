using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.UI.Helpers;
using ONTHERUSH.UI.Models;

namespace ONTHERUSH.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReporteController : Controller
    {
        // GET: ReporteController

        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService service)
        {
            _reporteService = service;
        }

        public IActionResult Index()
        {
            return View();
        }


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
                vm.Viajes = _reporteService.ObtenerReporteViaje(fechaInicio.Value, fechaFin.Value);
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Usuarios()
        {

            var vm = new ReporteUsuariosVM
             {
                 FechaInicio = DateTime.Now.AddDays(-30),
                 FechaFin = DateTime.Now
             };

            vm.Usuarios = _reporteService.ObtenerUsuarios((DateTime)vm.FechaInicio, (DateTime)vm.FechaFin);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Vehiculos()
        {
            var vm = new ReporteVehiculoVM();
            vm.Vehiculos = _reporteService.ObtenerVehiculos();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Reservas(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var vm = new ReporteReservaVM
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            vm.Reservas = _reporteService.ObtenerReservas(fechaInicio, fechaFin);

            return View(vm);
        }

        [HttpGet]
        public IActionResult ExportarPdfViajes(DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = _reporteService.ObtenerReporteViaje(fechaInicio, fechaFin);
            var pdf = PdfHelper.GenerarReporteViajes(datos);

            return File(pdf, "application/pdf", "ReporteViajes.pdf");
        }

        public IActionResult ExportarPdfUsuarios()
        {
            var fechaFin = DateTime.Now;
            var fechaInicio = fechaFin.AddDays(-30);
            
            var datos = _reporteService.ObtenerUsuarios(fechaInicio, fechaFin);
            var pdf = PdfHelper.GenerarReporteUsuarios(datos);

            return File(pdf, "application/pdf", "ReporteUsuarios.pdf");
        }

        public IActionResult ExportarVehiculosPdf()
        {
            var datos = _reporteService.ObtenerVehiculos();
            var pdf = PdfHelper.GenerarReporteVehiculos(datos);

            return File(pdf, "application/pdf", "ReporteVehiculos.pdf");
        }

        public IActionResult ExportarReservasPdf(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var datos = _reporteService.ObtenerReservas(fechaInicio, fechaFin);
            var pdf = PdfHelper.GenerarReporteReservas(datos);

            return File(pdf, "application/pdf", "ReporteReservas.pdf");
        }
    }
}
