using AutoMapper;
using back_end_Peliculas.DTOs;
using back_end_Peliculas.Entidades;
using back_end_Peliculas.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Controllers
{
    [Route("api/actores")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")] // para que solo usuario administrador puedan usar el endpoint
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";
        public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var actores = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync(); // asincrono 
            return mapper.Map<List<ActorDTO>>(actores);
        }
        //obener por id el actor para editar

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>>Get(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id); // await asincron
            if (actor == null)
            {
                return NotFound();
            }
            return mapper.Map<ActorDTO>(actor);

        }
        [HttpPost("buscarPorNombre")]
        public async Task<ActionResult<List<PeliculaActorDTO>>> BuscarPorNombre([FromBody] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) // si es nulo o vacio
            {
                return new List<PeliculaActorDTO>();  //me retonar una nueva lista
            }
            return await context.Actores.Where(x => x.Nombre.Contains(nombre))
                .Select(x => new PeliculaActorDTO { id = x.Id, Nombre = x.Nombre, Foto = x.Foto })
                .Take(5) // solo me va a traer 5 actores
                .ToListAsync();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            actor = mapper.Map(actorCreacionDTO, actor);
            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.EditarArchivo(contenedor, actorCreacionDTO.Foto, actor.Foto);
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO) // cambiamos FromBody por FromForm para enviar la foto
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);
            if (actorCreacionDTO.Foto != null)
            {
               actor.Foto = await almacenadorArchivos.GuardarArchivo(contenedor, actorCreacionDTO.Foto); // guardar goto
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }
        

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            context.Remove(actor);
            await context.SaveChangesAsync();
            await almacenadorArchivos.BorrarArchivo(actor.Foto, contenedor); // borra la foto
            return NoContent();
        }
    }
}
