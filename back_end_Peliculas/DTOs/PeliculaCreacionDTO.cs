using back_end_Peliculas.Utilidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.DTOs
{
    public class PeliculaCreacionDTO
    {
        [Required]
        [StringLength(maximumLength: 300)]
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public IFormFile Poster { get; set; } // IFormFile xq del front ende vamos a querer recibir un archivo
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))] // <List<int> es el parametro genérico
        public List<int> GenerosIds { get; set; } // ahi ya podemos recibir un listado fromform
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))] // <List<int> es el parametro genérico
        public List<int> CinesIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculaCreacionDTO>>))]
        public List<ActorPeliculaCreacionDTO> Actores { get; set; }
    }
}
