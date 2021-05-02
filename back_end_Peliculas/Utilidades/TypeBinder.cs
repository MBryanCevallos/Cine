using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Utilidades
{
    public class TypeBinder<T> : IModelBinder // clic derecho para implemntar la interfaz // <T> parametro generico
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;
            var valor = bindingContext.ValueProvider.GetValue(nombrePropiedad);
            if (valor == ValueProviderResult.None) // si no hay ningun valor
            {
                return Task.CompletedTask; // tarea completada y o hace nada
            }
            try
            {
                var valorDeserealziado = JsonConvert.DeserializeObject<T>(valor.FirstValue);// Newtonsoft hay que instalar  // <T> parametro generico
                bindingContext.Result = ModelBindingResult.Success(valorDeserealziado);
            }
            catch (Exception)
            {
                bindingContext.ModelState.TryAddModelError(nombrePropiedad, "El valor dado no es del tipo adecuado");
            }
            return Task.CompletedTask;
            
        }
    }
}
