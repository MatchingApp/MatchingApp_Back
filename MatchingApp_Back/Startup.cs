using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchingApp_Back.Models;

namespace MatchingApp_Back
{
    public class Startup
    {
        readonly string MyPolicy = "_MyPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            var settings = new ConnectionSettings()
                .DefaultMappingFor<Candidat>(x =>x.IndexName("candidats"));
            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyPolicy,
                                  builder =>
                                  {

                                      builder
                                         .AllowAnyOrigin()
                                         .AllowAnyMethod()
                                         .AllowAnyHeader();
                                  });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(MyPolicy);


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "CandidatController",
                    pattern: "CandidatController");
            });

        }
    }
}
