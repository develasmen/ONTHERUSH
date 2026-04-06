using System.ComponentModel.DataAnnotations;

namespace ONTHERUSH.Abstracciones.DTOs
{
    public class CrearIncidenteDto
    {
        [Required(ErrorMessage = "Debe seleccionar un tipo de incidente")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripción")]
        public string Descripcion { get; set; }
    }
}