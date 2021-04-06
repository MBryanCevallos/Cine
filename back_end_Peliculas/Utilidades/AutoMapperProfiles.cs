using AutoMapper;
using back_end_Peliculas.DTOs;
using back_end_Peliculas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Utilidades
{
    public class AutoMapperProfiles: Profile // heredo de automapper 
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap(); // reverseMap es de doble via es decir mapea desde dto a genero y de genero a dto
            CreateMap<GeneroCreacioDTO, Genero>(); // DTO hacia genero
        }
    }
}
