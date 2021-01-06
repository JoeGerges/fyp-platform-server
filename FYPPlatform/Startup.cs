using FYPPlatform.Web;
using FYPPlatform.Web.Middlewares;
using FYPPlatform.Web.Services;
using FYPPlatform.Web.Storage;
using FYPPlatform.Web.Storage.DocumentDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace FYPPlatform.Web
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

            //services.AddControllersWithViews()
            //    .AddNewtonsoftJson(options =>
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //);

            services.AddSingleton<IAccountsService, AccountsService>();
            services.AddSingleton<IAccountStore, DocumentDbAccountStore>();



            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.Configure<DocumentDbSettings>(Configuration.GetSection(nameof(DocumentDbSettings)));

            services.AddSingleton<IDocumentClient>(sp =>
            {
                var settings = GetSettings<DocumentDbSettings>();
                return new DocumentClient(new Uri(settings.EndpointUrl), settings.PrimaryKey,
                    new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp,
                        MaxConnectionLimit = settings.MaxConnectionLimit
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();

            app.UseMiddleware<ExceptionMiddleware>();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        private T GetSettings<T>() where T : new()
        {
            var config = Configuration.GetSection(typeof(T).Name);
            T settings = new T();
            config.Bind(settings);
            return settings;
        }
    }
}
