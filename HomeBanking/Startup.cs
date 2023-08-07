using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace HomeBanking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)  //Inyecci�n de dependencias, componentes disponibles p ser usados por la app
        {
            services.AddRazorPages(); //permite utilizar p�ginas razor en la app c#+html

            services.AddControllers().AddJsonOptions(x =>

             x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve); //permite que los controladores respondan a las solicitudes http

            // configuramos y agregamos un contexto de base de datos, disponible para cuando se necesite 
            services.AddDbContext<HomeBankingContext>(options => options.UseSqlServer(Configuration.GetConnectionString("HomeBankingConnection")));

            services.AddScoped<IClientRepository, ClientRepository>();

            services.AddScoped<IAccountRepository, AccountRepository>();

            //autenticaci�n
            //
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//m�todo de aunteticaci�n, nos permite utilizarr add.cookies
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10); 
                options.LoginPath = new PathString("/index.html"); //a donde queremos q nos dirija cdo hacemos el login
            });

            //autorizaci�n - quien puede acceder, hacer solicitudes al backend
            //
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client")); //solo dejamos ingresar a los q sean clientes
            });

            services.AddAuthorization(options =>

            {

                options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));

            });





        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)  //configura como se manejar�n las solicitudes HTTP entrantes y las respuestas salientes.
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            //le decimos que use autenticaci�n(utiliza la configuraciones que realizamos en serviecs)
            app.UseAuthentication();

            //autorizaci�n
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>   //puntos de acceso que tendr� nuestra app
            {
                endpoints.MapRazorPages();   //archivos con c�digo C# + HTML
                endpoints.MapControllers();  //agrega a los enpoints todas las clases que extiendan de controllers para q podamos acceder a ellas
            });
        }
    }
}
