using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.DTOs
{
    public class PeliculaActorDTO
    {
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
        public string Personaje { get; set; }
        public int Orden { get; set; } // con prop y tab
    }
}
