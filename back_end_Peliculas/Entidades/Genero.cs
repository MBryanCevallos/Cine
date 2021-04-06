using back_end_Peliculas.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Entidades
{
    public class Genero
    {
        // este remplazará a la tabla genero
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMayuscula]// validacion por atributo
        public string Nombre { get; set; }
        //[Range(18, 120)]
        //public int Edad { get; set; }
        //[CreditCard]
        //public string TarjetaDeCredito { get; set; }
        //[Url]
        //public string Url { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (!string.IsNullOrEmpty(Nombre))
        //    {
        //        var primeraLetra = Nombre[0].ToString();
        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser may+uscula",
        //                new string[] { nameof(Nombre) });
        //        }
        //    }
        //}
    }
}