using back_end_Peliculas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Repositorios
{
   public interface IRepositorio
    {
        void CrearGenero(Genero genero);
        Guid ObtenerGuid();

        //Genero ObtenerPorId(int id);
        //asincrono
        Task<Genero> ObtenerPorId(int id);
        List<Genero> ObtenerTodosLosGeneros();
    }
}
