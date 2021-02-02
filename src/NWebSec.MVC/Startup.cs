using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NWebSec.MVC
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
            services.AddControllersWithViews();
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
                app.UseHsts(opt => opt.MaxAge(365).Preload());
                app.UseXContentTypeOptions();
                app.UseXXssProtection(options => options.EnabledWithBlockMode());
                app.UseXfo(options => options.SameOrigin());
                app.UseReferrerPolicy(opts => opts.NoReferrerWhenDowngrade());
                app.UseCsp(opts => opts
                        .BlockAllMixedContent()
                        .StyleSources(s => s.Self())
                        .StyleSources(s => s.UnsafeInline())
                        .FontSources(s => s.Self())
                        .FormActions(s => s.Self())
                        .FrameAncestors(s => s.Self())
                        .ImageSources(s => s.Self())
                        .ScriptSources(s => s.Self())
                        );
                //Feature-Policy
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Feature-Policy",
                                                 "geolocation 'none';midi 'none';notifications 'none';push 'none';sync-xhr 'none';microphone 'none';camera 'none';magnetometer 'none';gyroscope 'none';speaker 'self';vibrate 'none';fullscreen 'self';payment 'none';");
                    await next.Invoke();
                });
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
