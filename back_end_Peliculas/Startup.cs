using AutoMapper;
using back_end_Peliculas.Controllers;
using back_end_Peliculas.Filtros;
using back_end_Peliculas.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_end_Peliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup)); // libreria automapper
            services.AddSingleton(provider =>
                new MapperConfiguration(config =>
                {
                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutoMapperProfiles(geometryFactory));
                }).CreateMapper());
            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326)); //nettopologySuite para captutar la posicion en el espacio


            //services.AddTransient<IAlmacenadorArchivos, AlmacenadorAzureStorage>(); // servicio para azure storage para foto en azure
            services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>(); // para foto localmente
            
            services.AddHttpContextAccessor(); // para foto localmente - tamien se debe agreagr

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"),
            SqlServer => SqlServer.UseNetTopologySuite() // nettopologySuite para usar
            ));

          
            services.AddCors(options =>
            {
                var frontend_Url = Configuration.GetValue<string>("frontend_url"); // tomamos la url del config
            options.AddDefaultPolicy(builder => //comunicacion cors entre dominios para ello habiltamos //este es el error  https://localhost:44385/api/generos (razón: falta la cabecera CORS 'Access-Control-Allow-Origin') 
            {
                builder.WithOrigins(frontend_Url).AllowAnyMethod().AllowAnyHeader()
                .WithExposedHeaders(new string[] { "cantidadtotalRegistros" }); //activar cabeceras para paginacion para que el front end pueda visualizar
            });
           });

            //JWT cuando se implementa el identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
         
           
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer( opciones =>
                opciones.TokenValidationParameters = new TokenValidationParameters { 
                ValidateIssuer =false,
                ValidateAudience = false,
                ValidateLifetime = true, // tiempo de expiración token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                ClockSkew = TimeSpan.Zero //calculo del tiempo
                });

            // FIN JWT cuando se implementa el identity

            // para obtener rol de los claims
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdmin", policy => policy.RequireClaim("role", "admin"));
            });
            //

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

            app.UseStaticFiles(); // widdelWare archivos estaticos

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
