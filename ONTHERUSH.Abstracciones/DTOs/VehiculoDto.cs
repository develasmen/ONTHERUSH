namespace ONTHERUSH.Abstracciones.DTOs
{
    public class VehiculoDto
    {
        public int VehiculoId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Año { get; set; }
        public int CapacidadPasajeros { get; set; }
        public string Estado { get; set; } = "Activo";
        public DateTime FechaRegistro { get; set; }
    }
}