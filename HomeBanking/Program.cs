using HomeBanking.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBanking
{
    public class Program   //  Punto de inicio de la app  -   configuración y ejecución - configuramos lo q necesite el cliente para poder conectarse a nuestro servicio
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();   //creamos un host  - configura el entorno de ejecución y servivios necesarios para q la app funcione
            using(var scope = host.Services.CreateScope())  //alcance de nuestra app
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<HomeBankingContext>();   //inyectamos el contexto
                    DbInitializer.Initialize(context);
                }
                catch(Exception ex) 
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ha ocurrido un error al enviar la información a la base de datos");
                }

            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
