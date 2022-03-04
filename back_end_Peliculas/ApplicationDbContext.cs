using back_end_Peliculas.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas
{
    public class ApplicationDbContext : IdentityDbContext //DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        //api fluente // para crear las llaves de las tablas // override onMo y tab
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasActores>()
                .HasKey(x => new { x.ActorId, x.PeliculaId }); // es decir las llaves estará compuesta por los dos ID
            
            modelBuilder.Entity<PeliculasGeneros>()
               .HasKey(x => new { x.GeneroId, x.PeliculaId });
            
            modelBuilder.Entity<PeliculasCines>()
               .HasKey(x => new { x.CineID, x.PeliculaId });

            base.OnModelCreating(modelBuilder); // una vez aplicado el identityDbContext tenemos que asegurar que esta linea esté
        }
        public DbSet<Genero> Generos { get; set; } // DbSet es par indicar que tablas quiero en mi BDD / calse Genero y nombre de la tabla Generos
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Cine> Cines { get; set; }

        public DbSet<Pelicula> Peliculas { get; set; } // nombre de la tabla en la bdd
        public DbSet<PeliculasActores> PeliculasActores { get; set; } // nombre de la tabla en la bdd
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; } // nombre de la tabla en la bdd
        public DbSet<PeliculasCines> PeliculasCines { get; set; } // nombre de la tabla en la bdd
        public DbSet<Rating> Rating { get; set; } // rating me gustas votos

    }
}
