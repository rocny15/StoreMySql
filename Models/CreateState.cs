using System.ComponentModel.DataAnnotations;

namespace StoreMySql.Models
{
    public class CreateState
    {
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(50, ErrorMessage ="El campo es demciado largo.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(3, ErrorMessage = "El campo es demciado largo.")]
        public string? Initials { get; set; }
    }
}
