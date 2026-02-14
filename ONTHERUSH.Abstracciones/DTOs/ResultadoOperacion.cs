namespace ONTHERUSH.Abstracciones.DTOs

{
    public class ResultadoOperacion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Datos { get; set; }
    }
}