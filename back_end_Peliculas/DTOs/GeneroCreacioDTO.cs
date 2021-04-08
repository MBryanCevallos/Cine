using back_end_Peliculas.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.DTOs
{
    public class GeneroCreacioDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100)]
        [PrimeraLetraMayuscula]// validacion por atributo
        public string Nombre { get; set; }
    }
}
