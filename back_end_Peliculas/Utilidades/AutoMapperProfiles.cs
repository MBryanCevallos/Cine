using AutoMapper;
using back_end_Peliculas.DTOs;
using back_end_Peliculas.Entidades;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Utilidades
{
    public class AutoMapperProfiles: Profile // heredo de automapper 
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap(); // reverseMap es de doble via es decir mapea desde dto a genero y de genero a dto
            CreateMap<GeneroCreacioDTO, Genero>(); // DTO hacia genero

            CreateMap<Actor, ActorDTO>().ReverseMap(); 
            CreateMap<ActorCreacionDTO, Actor>().ForMember(x => x.Foto, options => options.Ignore()); //se va a ingnorar el campo foto 

            CreateMap<CineCreacionDTO, Cine>().ForMember(x => x.Ubicacion, x => x.MapFrom(dto =>
            geometryFactory.CreatePoint(new Coordinate(dto.Longitud, dto.Latitud))));

            CreateMap<Cine, CineDTO>().ForMember(x => x.Latitud, dto => dto.MapFrom(campo => campo.Ubicacion.Y))
                .ForMember(x => x.Longitud, dto => dto.MapFrom(campo => campo.Ubicacion.X));
        }
    }
}
