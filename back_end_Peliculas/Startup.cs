using back_end_Peliculas.Controllers;
using back_end_Peliculas.Filtros;
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
            services.AddCors(options =>
            {
                var frontend_Url = Configuration.GetValue<string>("frontend_url"); // tomamos la url del config
            options.AddDefaultPolicy(builder => //comunicacion cors entre dominios para ello habiltamos //este es el error  https://localhost:44385/api/generos (raz�n: falta la cabecera CORS 'Access-Control-Allow-Origin') 
            {
                builder.WithOrigins(frontend_Url).AllowAnyMethod().AllowAnyHeader(); 
            });
        });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddControllers(options =>{
                options.Filters.Add(typeof(FiltroDeExcepcion));// aqui voy a agregar mi filtro global
            }); 
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "back_end_Peliculas", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "back_end_Peliculas v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(); // middleware para la comunicacio de dominiios en proyectos web

            app.UseAuthentication(); // antes de autorizacion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
