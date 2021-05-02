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
    [Route("api/peliculas")]
    public class PeliculasController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context, 
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos) // para el poster
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
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

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
    {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);
            if (peliculaCreacionDTO.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionDTO.Poster); // guardar poster
            }
            EscribirOrdebdeActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return NoContent();
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
    }
}
