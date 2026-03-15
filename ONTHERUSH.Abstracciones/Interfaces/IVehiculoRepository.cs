using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.Abstracciones.Interfaces
{
    public interface IVehiculoRepository
    {
        Task<List<VehiculoDto>> ObtenerTodos();
        Task<VehiculoDto?> ObtenerPorId(int id);
        Task<ResultadoOperacion> Agregar(VehiculoDto vehiculo);
        Task<ResultadoOperacion> Actualizar(VehiculoDto vehiculo);
        Task<ResultadoOperacion> Eliminar(int id);
        Task<bool> ExistePlaca(string placa, int? vehiculoId = null);
    }
}