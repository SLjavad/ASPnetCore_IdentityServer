using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspIdentityServer.IdentitySvr.Configurations;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspIdentityServer.IdentitySvr
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            string constr = Configuration.GetConnectionString("DbConnectionString");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddInMemoryIdentityResources(Config.GetAllIdentityResources())
                //.AddInMemoryApiResources(Config.GetAllApiResources())
                //.AddInMemoryClients(Config.GetAllClients())
                .AddTestUsers(Config.GetAllUsers())
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = ctx => ctx.UseNpgsql(constr, opt => opt.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = ctx => ctx.UseNpgsql(constr, opt => opt.MigrationsAssembly(migrationAssembly));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            SeedIdentityDb(app);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapDefaultControllerRoute();
            });

        }

        private void SeedIdentityDb(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var cnfContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                cnfContext.Database.Migrate();

                if (!cnfContext.Clients.Any())
                {
                    foreach (var client in Config.GetAllClients())
                    {
                        cnfContext.Clients.Add(client.ToEntity());
                    }
                    cnfContext.SaveChanges();
                }

                if (!cnfContext.IdentityResources.Any())
                {
                    foreach (var identityResource in Config.GetAllIdentityResources())
                    {
                        cnfContext.IdentityResources.Add(identityResource.ToEntity());
                    }
                    cnfContext.SaveChanges();
                }

                if (!cnfContext.ApiResources.Any())
                {
                    foreach (var apiResource in Config.GetAllApiResources())
                    {
                        cnfContext.ApiResources.Add(apiResource.ToEntity());
                    }
                    cnfContext.SaveChanges();
                }
            }
        }
    }
}
