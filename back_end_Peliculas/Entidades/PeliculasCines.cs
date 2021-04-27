using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Entidades
{
    public class PeliculasCines
    {
        public int PeliculaId { get; set; }
        public int CineID { get; set; }
        public Pelicula Pelicula { get; set; }
        public Cine Cine { get; set; }

    }
}
