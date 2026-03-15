using Microsoft.EntityFrameworkCore;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.AccesoADatos.Repositories
{
    public class VehiculoRepository : IVehiculoRepository
    {
        private readonly ApplicationDbContext _context;

        public VehiculoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehiculoDto>> ObtenerTodos()
        {
            var vehiculos = await _context.Vehiculos
                .OrderByDescending(v => v.FechaRegistro)
                .ToListAsync();

            return vehiculos.Select(v => new VehiculoDto
            {
                VehiculoId = v.VehiculoId,
                Placa = v.Placa,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Año = v.Año,
                CapacidadPasajeros = v.CapacidadPasajeros,
                Estado = v.Estado,
                FechaRegistro = v.FechaRegistro
            }).ToList();
        }

        public async Task<VehiculoDto?> ObtenerPorId(int id)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.VehiculoId == id);

            if (vehiculo == null) return null;

            return new VehiculoDto
            {
                VehiculoId = vehiculo.VehiculoId,
                Placa = vehiculo.Placa,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Año = vehiculo.Año,
                CapacidadPasajeros = vehiculo.CapacidadPasajeros,
                Estado = vehiculo.Estado,
                FechaRegistro = vehiculo.FechaRegistro
            };
        }

        public async Task<ResultadoOperacion> Agregar(VehiculoDto vehiculoDto)
        {
            try
            {
                var vehiculo = new Vehiculo
                {
                    Placa = vehiculoDto.Placa,
                    Marca = vehiculoDto.Marca,
                    Modelo = vehiculoDto.Modelo,
                    Año = vehiculoDto.Año,
                    CapacidadPasajeros = vehiculoDto.CapacidadPasajeros,
                    Estado = vehiculoDto.Estado,
                    FechaRegistro = DateTime.Now
                };

                _context.Vehiculos.Add(vehiculo);
                await _context.SaveChangesAsync();

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Vehículo agregado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al agregar el vehículo: {ex.Message}"
                };
            }
        }

        public async Task<ResultadoOperacion> Actualizar(VehiculoDto vehiculoDto)
        {
            try
            {
                var vehiculo = await _context.Vehiculos
                    .FirstOrDefaultAsync(v => v.VehiculoId == vehiculoDto.VehiculoId);

                if (vehiculo == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje = "Vehículo no encontrado"
                    };
                }

                vehiculo.Placa = vehiculoDto.Placa;
                vehiculo.Marca = vehiculoDto.Marca;
                vehiculo.Modelo = vehiculoDto.Modelo;
                vehiculo.Año = vehiculoDto.Año;
                vehiculo.CapacidadPasajeros = vehiculoDto.CapacidadPasajeros;
                vehiculo.Estado = vehiculoDto.Estado;

                _context.Vehiculos.Update(vehiculo);
                await _context.SaveChangesAsync();

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Vehículo actualizado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al actualizar el vehículo: {ex.Message}"
                };
            }
        }

        public async Task<ResultadoOperacion> Eliminar(int id)
        {
            try
            {
                var vehiculo = await _context.Vehiculos
                    .FirstOrDefaultAsync(v => v.VehiculoId == id);

                if (vehiculo == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje = "Vehículo no encontrado"
                    };
                }

                _context.Vehiculos.Remove(vehiculo);
                await _context.SaveChangesAsync();

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = "Vehículo eliminado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje = $"Error al eliminar el vehículo: {ex.Message}"
                };
            }
        }

        public async Task<bool> ExistePlaca(string placa, int? vehiculoId = null)
        {
            if (vehiculoId.HasValue)
            {
                return await _context.Vehiculos
                    .AnyAsync(v => v.Placa == placa && v.VehiculoId != vehiculoId.Value);
            }

            return await _context.Vehiculos.AnyAsync(v => v.Placa == placa);
        }
    }
}