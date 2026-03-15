using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IVehiculoService
    {
        Task<List<VehiculoDto>> ObtenerVehiculos();
        Task<VehiculoDto?> ObtenerPorId(int id);
        Task<ResultadoOperacion> AgregarVehiculo(VehiculoDto vehiculo);
        Task<ResultadoOperacion> ActualizarVehiculo(VehiculoDto vehiculo);
        Task<ResultadoOperacion> EliminarVehiculo(int id);
    }
}