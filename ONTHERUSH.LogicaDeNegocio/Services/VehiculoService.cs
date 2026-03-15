using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class VehiculoService : IVehiculoService
    {
        private readonly IVehiculoRepository _vehiculoRepository;

        public VehiculoService(IVehiculoRepository vehiculoRepository)
        {
            _vehiculoRepository = vehiculoRepository;
        }

        public async Task<List<VehiculoDto>> ObtenerVehiculos()
        {
            return await _vehiculoRepository.ObtenerTodos();
        }

        public async Task<VehiculoDto?> ObtenerPorId(int id)
        {
            return await _vehiculoRepository.ObtenerPorId(id);
        }

        public async Task<ResultadoOperacion> AgregarVehiculo(VehiculoDto vehiculo)
        {
            // Validar placa única
            if (await _vehiculoRepository.ExistePlaca(vehiculo.Placa))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Ya existe un vehículo con esa placa"
                };
            }

            // Validar año
            if (vehiculo.Año < 1900 || vehiculo.Año > DateTime.Now.Year + 1)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "El año del vehículo no es válido"
                };
            }

            // Validar capacidad
            if (vehiculo.CapacidadPasajeros <= 0 || vehiculo.CapacidadPasajeros > 50)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "La capacidad debe estar entre 1 y 50 pasajeros"
                };
            }

            return await _vehiculoRepository.Agregar(vehiculo);
        }

        public async Task<ResultadoOperacion> ActualizarVehiculo(VehiculoDto vehiculo)
        {
            // Validar que existe
            var vehiculoExistente = await _vehiculoRepository.ObtenerPorId(vehiculo.VehiculoId);
            if (vehiculoExistente == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Vehículo no encontrado"
                };
            }

            // Validar placa única (excepto el mismo vehículo)
            if (await _vehiculoRepository.ExistePlaca(vehiculo.Placa, vehiculo.VehiculoId))
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Ya existe otro vehículo con esa placa"
                };
            }

            // Validar año
            if (vehiculo.Año < 1900 || vehiculo.Año > DateTime.Now.Year + 1)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "El año del vehículo no es válido"
                };
            }

            // Validar capacidad
            if (vehiculo.CapacidadPasajeros <= 0 || vehiculo.CapacidadPasajeros > 50)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "La capacidad debe estar entre 1 y 50 pasajeros"
                };
            }

            return await _vehiculoRepository.Actualizar(vehiculo);
        }

        public async Task<ResultadoOperacion> EliminarVehiculo(int id)
        {
            var vehiculo = await _vehiculoRepository.ObtenerPorId(id);
            if (vehiculo == null)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = "Vehículo no encontrado"
                };
            }

            return await _vehiculoRepository.Eliminar(id);
        }
    }
}