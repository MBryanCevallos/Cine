using AutoMapper;
using back_end_Peliculas.DTOs;
using back_end_Peliculas.Entidades;
using back_end_Peliculas.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Controllers
{
    [ApiController]
    [Route("api/cines")]
    public class CinesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CinesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet] //Para mostrar todos
        public async Task<ActionResult<List<CineDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO) //tambien puedo usar ations result en una lista
        {
            var queryable = context.Cines.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var cines = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync(); // asincrono 
            return mapper.Map<List<CineDTO>>(cines);

        }
        [HttpGet("{Id:int}")] // para obtener por el id para luego edite con el put
        public async Task<ActionResult<CineDTO>> Get(int Id) // actionResult es para que funcione notfound() = 404  // async task y a wait es para usar un metodo asincrono   // BindRequired parametro obligatorio // no obligatorio FromHeader
        {
            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == Id);
            if (cine == null)
            {
                return NotFound(); // retorna un 404
            }
            return mapper.Map<CineDTO>(cine);
        }
        [HttpPut("{Id:int}")] // put para editar
        public async Task<ActionResult> Put(int Id, [FromBody] CineCreacionDTO cineCreacionDTO) //FromBody se usa para formularios generalmente en post y put
        {
            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == Id);
            if (cine == null)
            {
                return NotFound(); // retorna un 404
            }
            cine = mapper.Map(cineCreacionDTO, cine);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost] // Crear uno nuevo
        public async Task<ActionResult> Post([FromBody] CineCreacionDTO cineCreacionDTO)
        {
            var cine = mapper.Map<Cine>(cineCreacionDTO);
            context.Add(cine);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")] //Eliminar
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Cines.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Cine() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
