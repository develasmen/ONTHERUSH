using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class ViajeService : IViajeService
    {
        private readonly IViajeRepository _viajeRepository;

        public ViajeService(IViajeRepository viajeRepository)
        {
            _viajeRepository = viajeRepository;
        }

        public async Task<ResultadoOperacion> CrearViaje(
            int conductorId,
            int asignadoPorAdminId,
            int vehiculoId,
            string nombreRuta,
            string puntoPartida,
            string? paradas,
            DateTime fechaHoraSalida,
            DateTime horaEstimadaLlegada,
            int cantidadPasajeros,
            string? observaciones
        )
        {
            var viaje = new Viaje
            {
                ConductorId = conductorId,
                AsignadoPorAdminId = asignadoPorAdminId,
                VehiculoId = vehiculoId,
                NombreRuta = nombreRuta,
                PuntoPartida = puntoPartida,
                Paradas = paradas,
                FechaHoraSalida = fechaHoraSalida,
                HoraEstimadaLlegada = horaEstimadaLlegada,
                CantidadPasajeros = cantidadPasajeros,
                Observaciones = observaciones,
                Estado = "Programado",
                FechaAsignacion = DateTime.Now,
                FechaCreacion = DateTime.Now
            };

            return await _viajeRepository.CrearViaje(viaje);
        }

        public async Task<object?> ObtenerViajePorId(int viajeId)
        {
            return await _viajeRepository.ObtenerViajePorId(viajeId);
        }

        public async Task<ResultadoOperacion> FinalizarViaje(int viajeId)
        {
            return await _viajeRepository.FinalizarViaje(viajeId);
        }
    }
}