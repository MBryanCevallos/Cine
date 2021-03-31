using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Filtros
{
    public class MiFiltroDeAccion : IActionFilter // control . para implementar interfaz
    {
        private readonly ILogger<MiFiltroDeAccion> logger;

        public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> logger) //ctor //  clic derecho en logger accion - asignar como un campo
        {
            this.logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("antes de ejecuta la acción");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("después de ejecuta la acción");
        }

     
    }
}
