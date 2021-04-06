using AutoMapper;
using back_end_Peliculas.DTOs;
using back_end_Peliculas.Entidades;
using back_end_Peliculas.Filtros;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Controllers
{
    [Route("api/generos")] //endpoint, también podría estar "api/[controller]" hola
    [ApiController] // para no usar en cada metodo http adRequest(ModelState); para cuando haya un error
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // autotizacion de autenticacion a nivel de todo el controlador
    public class GenerosController : ControllerBase //metodo auxiliar
    {
        private readonly ILogger<GenerosController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(            
            ILogger<GenerosController> logger // inyectamos iLOGGER)  y click derencho iniciarliazar como un campo
            , ApplicationDbContext context // asignar como un campo clic derecho
            , IMapper mapper)  
        {
           
            this.logger = logger;  //iniciailazamos logger como un campo 
            this.context = context; // se crea aqui
            this.mapper = mapper;
        }

        //accion que se ejcuta cuando se hace una petición http al end point
        [HttpGet] //metodo http get
        public async Task<ActionResult<List<GeneroDTO>>> Get() //tambien puedo usar ations result en una lista
        {
            // return new List<Genero>() { new Genero() { Id = 1, Nombre = "Comedia" } };
            //return await context.Generos.ToListAsync(); // asincrono
            var generos = await context.Generos.ToListAsync();
            return mapper.Map<List<GeneroDTO>>(generos); // automapper 

        }
        //[HttpGet("{id}")]//quedaría asi "api/generos/id" y mediante postman enviamos en el query string el id quedando asi https://localhost:44385/api/generos/id?id=2 o podemos directamente enviar la variable en la urlasi "{id}  quedandon asi https://localhost:44385/api/generos/1"
        //[HttpGet("{id}/{nombre}")]// ejemplo con mas de un parámetro quedaría asi "api/generos/1/marlon
        //[HttpGet("{id:int}/{nombre=marlon}")]// ejemplo cuando se requiere definir el tipo de parametro {id: int} y tambien de puede enviar el valor del paramtro por defecto {nombre=marlon}
                                             // public Genero Get(int id, string nombre) // retorna una acción especifica
        [HttpGet("{Id:int}")]
        public async Task<ActionResult<Genero>> Get(int Id) // actionResult es para que funcione notfound() = 404  // async task y a wait es para usar un metodo asincrono   // BindRequired parametro obligatorio // no obligatorio FromHeader
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacioDTO generoCreacionDto) // se cambia el void por action result asi public void Post -- retorna un 404
        {
            var genero = mapper.Map<Genero>(generoCreacionDto);
            //throw new NotImplementedException();
            context.Add(genero);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut]
        public ActionResult Put([FromBody] Genero genero) //FromBody se usa para formularios generalmente en post y put
        {
            throw new NotImplementedException();
        }
        [HttpDelete]
        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}
