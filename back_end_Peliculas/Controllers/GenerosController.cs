using back_end_Peliculas.Entidades;
using back_end_Peliculas.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Controllers
{
    [Route("api/generos")] //endpoint, también podría estar "api/[controller]"
    [ApiController] // para no usar en cada metodo http adRequest(ModelState); para cuando haya un error
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // autotizacion de autenticacion
    public class GenerosController: ControllerBase //metodo auxiliar
    {
        private readonly IRepositorio repositorio;
        private readonly WeatherForecastController weatherForecastController;
        private readonly ILogger<GenerosController> logger;

        public GenerosController(IRepositorio repositorio, 
            WeatherForecastController weatherForecastController,//Control. crea y asgina repositorio
            ILogger <GenerosController> logger) // inyectamos iLOGGER)  y click derencho iniciarliazar como un campo
        {
            this.repositorio = repositorio;
            this.weatherForecastController = weatherForecastController;
            this.logger = logger;  //iniciailazamos logger como un campo 

        }

        //accion que se ejcuta cuando se hace una petición http al end point
        [HttpGet] //metodo http get
        [HttpGet("listado")] // quedaria /api/generos/listado y tomaria este endpoint como el de arriba api/generos
        [HttpGet("/listadogeneros")]  //quedarias asi /listadogeneros xq el / no toma en cuenta este api/generos
        //[ResponseCache(Duration =60)] //tiempo de duraci{on del caché  - capa de caché
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // autotizacion de autenticacion
        public ActionResult<List<Genero>> Get() //tambien puedo usar ations result en una lista
        {
            logger.LogInformation("vamos a mostrar los gérneros");
            return repositorio.ObtenerTodosLosGeneros();
        }
        //[HttpGet("{id}")]//quedaría asi "api/generos/id" y mediante postman enviamos en el query string el id quedando asi https://localhost:44385/api/generos/id?id=2 o podemos directamente enviar la variable en la urlasi "{id}  quedandon asi https://localhost:44385/api/generos/1"
        //[HttpGet("{id}/{nombre}")]// ejemplo con mas de un parámetro quedaría asi "api/generos/1/marlon
        //[HttpGet("{id:int}/{nombre=marlon}")]// ejemplo cuando se requiere definir el tipo de parametro {id: int} y tambien de puede enviar el valor del paramtro por defecto {nombre=marlon}
                                             // public Genero Get(int id, string nombre) // retorna una acción especifica
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id, [FromHeader] string nombre) // actionResult es para que funcione notfound() = 404  // async task y a wait es para usar un metodo asincrono   // BindRequired parametro obligatorio // no obligatorio FromHeader
        {
            logger.LogDebug($"obtenidendo un género por el ID : {id}"); // $ es string inteporlation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }             

            var genero = await repositorio.ObtenerPorId(id);
            if (genero ==null)
            {
                logger.LogWarning($"No pudimos encontrar el género de ID {id}");
                return NotFound();
            }
            return genero;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genero genero) // se cambia el void por action result asi public void Post -- retorna un 404
        {
            repositorio.CrearGenero(genero);
            return NoContent();
        }
        [HttpPut]
        public ActionResult Put([FromBody] Genero genero) //FromBody se usa para formularios generalmente en post y put
        {
            return NoContent();
        }
        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }
        [HttpGet("guid")] // api/generos/guid
        public ActionResult<Guid> GetGUID() 
        {
            return Ok(new { GUID_GenerosController = repositorio.ObtenerGuid(),
                GUID_WeatherForecastController = weatherForecastController.ObtenerGUIDWeatherForecastController()});
        }
    }
}
