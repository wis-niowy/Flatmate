using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Flatmate.Models;
using AutoMapper;
using Flatmate.Data;
using System;
using Microsoft.AspNetCore.Identity;
using Flatmate.Models.EntityModels;

namespace Flatmate
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // konfiguracja serwisow dla zarzadzania sesja uzytkownika
            services.AddDistributedMemoryCache();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly     = true;
                // Make the session cookie essential
                options.Cookie.IsEssential  = true;
                options.Cookie.Name         = "FlatmateCookie";
                options.Cookie.Expiration   = TimeSpan.FromMinutes(15);
            });
            services.AddSession(options =>
            {
                //// Set a short timeout for easy testing.
                //options.IdleTimeout = TimeSpan.FromMinutes(15);
                //options.Cookie.HttpOnly = true;
                //// Make the session cookie essential
                //options.Cookie.IsEssential = true;
                //options.Cookie.Name = "FlatmateCookie";
            });
            

            services.AddAutoMapper();
            services.AddMemoryCache();
            
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
                });
            services.AddDbContext<FlatmateContext>
                (options => {
                    options.UseSqlServer(Configuration.GetConnectionString("LocalDBConnection"));
                });
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            
            //services.AddAuthe
            services.AddIdentity<User,IdentityRole<int>>() // wstrzyknięcie typu User do IdentityUserContext
                .AddEntityFrameworkStores<FlatmateContext>();
            //.AddDefaultUI(/*UIFramework.Bootstrap4*/)
                
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }

}

