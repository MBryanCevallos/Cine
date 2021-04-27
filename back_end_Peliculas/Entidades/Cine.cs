using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Entidades
{
    public class Cine
    {
        public int Id { get; set; } // prop
        [Required]
        [StringLength(maximumLength: 75)]
        public string Nombre { get; set; }
        public Point Ubicacion { get; set; }//querys espaciales - usa libreria nettopologySuite / point es para latitud y longitud
    }
}
