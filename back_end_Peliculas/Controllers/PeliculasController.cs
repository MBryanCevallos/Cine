using AutoMapper;
using back_end_Peliculas.DTOs;
using back_end_Peliculas.Entidades;
using back_end_Peliculas.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")] // para que solo usuario administrador puedan usar el endpoint
    public class PeliculasController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context, 
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IAlmacenadorArchivos almacenadorArchivos) // para el poster
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.almacenadorArchivos = almacenadorArchivos;
        }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<LandingPageDTO>> Get()
        {
            var top = 6;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(x => x.FechaLanzamiento > hoy)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var enCines = await context.Peliculas
                .Where(x => x.EnCines)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();
            var resultado = new LandingPageDTO();
            resultado.ProximosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

            return resultado;

        }


    [HttpGet("PostGet")] // es un get para luego realizar un post
    public async Task<ActionResult<PeliculasPostGetDTO>> PosGet()
        {
            var cines = await context.Cines.ToListAsync();
            var generos = await context.Generos.ToListAsync();

            var cinesDTO = mapper.Map<List<CineDTO>>(cines);
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return new PeliculasPostGetDTO()
            {
                Cines = cinesDTO,
                Generos = generosDTO
            };
        }

    [HttpGet("PutGet/{id:int}")] 
    //[AllowAnonymous]
    public async Task<ActionResult<PeliculasPutGetDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult)
            {
                return NotFound();
            }
            var pelicula = peliculaActionResult.Value;
            var generosSeleccionadorIds = pelicula.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos
                .Where(x => !generosSeleccionadorIds.Contains(x.Id))
                .ToListAsync();

            var cinesSeleccionadorIds = pelicula.Cines.Select(x => x.Id).ToList();
            var cinesNoSeleccionados = await context.Cines
                .Where(x => !cinesSeleccionadorIds.Contains(x.Id))
                .ToListAsync();

            var generosNoSeleccionadosDTO = mapper.Map<List<GeneroDTO>>(generosNoSeleccionados);
            var cinesNoSeleccionadosDTO = mapper.Map<List<CineDTO>>(cinesNoSeleccionados);

            var respuesta = new PeliculasPutGetDTO();
            respuesta.Pelicula = pelicula;
            respuesta.GenerosSeleccionados = pelicula.Generos;
            respuesta.GenerosNoSeleccionados = generosNoSeleccionadosDTO;
            respuesta.CinesSeleccionados = pelicula.Cines;
            respuesta.CinesNoSeleccionados = cinesNoSeleccionadosDTO;
            respuesta.Actores = pelicula.Actores;
            return respuesta;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasActores)
                .Include(x => x.PeliculasGeneros)
                .Include(x => x.peliculasCines)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }
            pelicula = mapper.Map(peliculaCreacionDTO, pelicula);
            if (peliculaCreacionDTO.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos.EditarArchivo(contenedor, peliculaCreacionDTO.Poster, pelicula.Poster);
            
            }
            EscribirOrdebdeActores(pelicula);

            await context.SaveChangesAsync();
            return NoContent();
        }
        
    [HttpGet("{id:int}")]
    [AllowAnonymous] // A pesar de ser autorizado se puede consultar
    public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.peliculasCines).ThenInclude(x => x.Cine)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            //¨Promedio y votaciones
            var promedioVoto = 0.0;
            var usuarioVoto = 0;
            if (await context.Rating.AnyAsync(x => x.PeliculaId == id))
            {
                promedioVoto = await context.Rating.Where(x => x.PeliculaId == id)
                    .AverageAsync(x => x.Puntuacion); // AverageAsync es para obtener el promedio
                if (HttpContext.User.Identity.IsAuthenticated) // compruebo que el usuario esté autenticado
                {
                    var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value; // obenter informacion del usu
                    var usuario = await userManager.FindByEmailAsync(email);
                    var usuarioId = usuario.Id;
                    var ratingDB = await context.Rating.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.PeliculaId == id);
                    if (ratingDB != null)
                    {
                        usuarioVoto = ratingDB.Puntuacion;
                    }

                }
                
            }

            var dto = mapper.Map<PeliculaDTO>(pelicula);
            dto.VotoUsuario = usuarioVoto;
            dto.PromedioVoto = promedioVoto;
            dto.Actores = dto.Actores.OrderBy(x => x.Orden).ToList();
            return dto;
        }
     
    [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] PeliculasFiltrarDTO peliculasFiltrarDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(peliculasFiltrarDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(peliculasFiltrarDTO.Titulo));
            }
            if (peliculasFiltrarDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }
            if (peliculasFiltrarDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaLanzamiento > hoy);
            }
            if (peliculasFiltrarDTO.GeneroId !=0)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                .Contains(peliculasFiltrarDTO.GeneroId));
            }

            await HttpContext.InsertarParametrosPaginacionEnCabecera(peliculasQueryable);
            var peliculas = await peliculasQueryable.Paginar(peliculasFiltrarDTO.PaginacionDTO).ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);

        }      

    [HttpPost]
    public async Task<ActionResult <int>> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);
            if (peliculaCreacionDTO.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionDTO.Poster); // guardar poster
            }
            EscribirOrdebdeActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id; //para retornar
        }
    
    private void EscribirOrdebdeActores(Pelicula pelicula) // obtener el orden en el que vinieron

        {
            if (pelicula.PeliculasActores != null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }
            context.Remove(pelicula);
            await context.SaveChangesAsync();
            await almacenadorArchivos.BorrarArchivo(pelicula.Poster, contenedor); // borra la foto
            return NoContent();
        }
    }
}
