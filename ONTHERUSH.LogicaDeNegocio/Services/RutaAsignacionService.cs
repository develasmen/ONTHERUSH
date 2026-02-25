using System.Collections.Concurrent;
using ONTHERUSH.Abstracciones.DTOs;

namespace ONTHERUSH.LogicaDeNegocio.Services
{
    public class RutaAsignacionService
    {
        private readonly ConcurrentDictionary<string, List<ParadaAsignadaDto>> _rutas
            = new ConcurrentDictionary<string, List<ParadaAsignadaDto>>();

        public void AsignarRuta(string conductorId, List<ParadaAsignadaDto> paradas)
        {
            foreach (var parada in paradas)
            {
                parada.Estado = "Pendiente";
            }

            _rutas[conductorId] = paradas
                .OrderBy(p => p.Orden)
                .ToList();
        }

        public List<ParadaAsignadaDto> ObtenerRuta(string conductorId)
        {
            if (_rutas.TryGetValue(conductorId, out var paradas))
            {
                return paradas;
            }

            return new List<ParadaAsignadaDto>();
        }

        public void LimpiarRuta(string conductorId)
        {
            _rutas.TryRemove(conductorId, out _);
        }

        public bool TieneRuta(string conductorId)
        {
            return _rutas.TryGetValue(conductorId, out var lista) && lista != null && lista.Count > 0;
        }
    }
}