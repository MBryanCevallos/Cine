using back_end_Peliculas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Repositorios
{
    public class RepositorioEnMemoria : IRepositorio //implemnta la interfaz IRepositorio
    {
        private List<Genero> _generos;
        public RepositorioEnMemoria()
        {
            _generos = new List<Genero>()
            {
                new Genero(){id =1, Nombre = "Comedia"},
                new Genero(){id =2, Nombre = "Acción"}
            };
            _guid = Guid.NewGuid(); // string aleatorio AdFGH-12345-QWRTYY
        }

        public Guid _guid;
        public List<Genero> ObtenerTodosLosGeneros()
        {
            return _generos;
        }

        //public Genero ObtenerPorId(int id)
        //{
        //    return _generos.FirstOrDefault(x => x.id == id); //landa firstOrDefault puedo traer un valor o null
        //}
        //asincrono
        public async Task<Genero> ObtenerPorId(int id)
        {
            //await Task.Delay(TimeSpan.FromSeconds(3)); //esperar 3 segundos de manera asincrona
            await Task.Delay(1); // lo mismo pero en milegundo
            return _generos.FirstOrDefault(x => x.id == id); //landa firstOrDefault puedo traer un valor o null
        }
        public Guid ObtenerGuid() {

            return _guid;
        }

        public void CrearGenero(Genero genero)
        {
            genero.id = _generos.Count() + 1;
            _generos.Add(genero);
        }

    }
}
