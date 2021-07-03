using CoffeeShop.ApiGateway.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.Middleware;
using System;
using System.Linq;

namespace CoffeeShop.ApiGateway
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var installers = typeof(Startup).Assembly.ExportedTypes
                                            .Where(x => typeof(IInstaller).IsAssignableFrom(x) &&
                                                    !x.IsInterface && !x.IsAbstract)
                                            .Select(Activator.CreateInstance)
                                            .Cast<IInstaller>().ToList();

            installers.ForEach(installer => installer.InstallServices(services, _configuration));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("API Gateway Running...");
                });
            });

            app.UseOcelot().Wait();
        }
    }
}