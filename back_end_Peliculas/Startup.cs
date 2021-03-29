using back_end_Peliculas.Controllers;
using back_end_Peliculas.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddResponseCaching(); // activicamos el caché
            //services.AddTransient<IRepositorio,RepositorioEnMemoria>(); //Siempre se retonarna distintantas instancias -configuracion de inyección de dependencias
            // services.AddSingleton<IRepositorio, RepositorioEnMemoria>(); //configuracion de inyección de dependencias
            services.AddScoped<IRepositorio, RepositorioEnMemoria>(); //configuracion de inyección de dependencias
            services.AddScoped<WeatherForecastController>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "back_end_Peliculas", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            ILogger<Startup> logger)
        {
            //por lo general los USE no terminan el proceso de middleware
            app.Use(async (context, next) =>  
            {
                using (var swapStream = new MemoryStream())
                {
                    var respuestaOriginal = context.Response.Body;
                    context.Response.Body = swapStream;
                    await next.Invoke();

                    swapStream.Seek(0, SeekOrigin.Begin);
                    string respuesta = new StreamReader(swapStream).ReadToEnd();
                    swapStream.Seek(0, SeekOrigin.Begin);

                    await swapStream.CopyToAsync(respuestaOriginal);
                    context.Response.Body = respuestaOriginal;

                    logger.LogInformation(respuesta); //guardamos en un log la respuesta http.
                }
            });
            app.Map("/mapa1", (app) =>   // permiti que la peticion http solo se haga cuando sea con /mapa1
            {
                app.Run(async context => { //terminar la peticion 

                    await context.Response.WriteAsync("Estoy intecertando el pipeline o tubería");
                });

            });
           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "back_end_Peliculas v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching(); // debe usarse antes del MapController

            app.UseAuthentication(); // antes de autorizacion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
