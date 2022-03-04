using back_end_Peliculas.DTOs;
using back_end_Peliculas.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Controllers
{
    [Route("api/rating")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext context;

        public RatingsController(UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // para validar que el usuario tiene permiso para cceder al endpoint
        public async Task<ActionResult> Post([FromBody] RatingDTO ratingDTO)
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var ratingActual = await context.Rating.FirstOrDefaultAsync(x => x.PeliculaId == ratingDTO.PeliculaId
            && x.UsuarioId == usuarioId);

            if (ratingActual == null) // si no ha votado
            {
                var rating = new Rating();
                rating.PeliculaId = ratingDTO.PeliculaId;
                rating.Puntuacion = ratingDTO.Puntuacion;
                rating.UsuarioId = usuarioId;
                context.Add(rating);
                
            }
            else
            {
                ratingActual.Puntuacion = ratingDTO.Puntuacion;
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
