using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FullDuplexServer.Abstractions;
using FullDuplexServer.Services;

namespace FullDuplexServer
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
            services.AddControllers();

            services.AddAuthorization();

            services.AddMemoryCache();

            services.AddHttpClient(RemoteTokenValidationService.HTTP_CLIENT_NAME, client =>
            {
                //from configuration
                client.BaseAddress = new Uri("http://localhost:1337/");
                client.DefaultRequestHeaders.Add("Authorization", "bebe jhgkdsjhgkjsdfhgk.shfdjkgkdhj.sdjfghksjd");
                client.Timeout = TimeSpan.FromMinutes(1);
            });

            services.AddSingleton<ITokenValidationService, RemoteTokenValidationService>();
            services.AddAuthentication(AuthHandler.SCHEME)
                .AddScheme<AuthHandlerOpt, AuthHandler>(AuthHandler.SCHEME, null);

            //services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
