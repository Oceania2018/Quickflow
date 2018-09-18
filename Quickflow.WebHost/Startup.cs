using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetToolkit.JwtHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using Quickflow.Core;
using Swashbuckle.AspNetCore.Swagger;

namespace Quickflow.WebHost
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddJwtAuth(Configuration);

            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer schema. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                  { "Bearer", Enumerable.Empty<string>() },
                });

                var info = Configuration.GetSection("Swagger").Get<Info>();
                c.SwaggerDoc(info.Version, info);

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Quickflow.RestApi.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var info = Configuration.GetSection("Swagger").Get<Info>();

                c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Patch, SubmitMethod.Delete);
                c.ShowExtensions();
                c.SwaggerEndpoint(Configuration.GetValue<String>("Swagger:Endpoint"), info.Title);
                c.RoutePrefix = "api";
                c.DocumentTitle = info.Title;
                c.InjectStylesheet(Configuration.GetValue<String>("Swagger:Stylesheet"));
            });

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());

            app.Use(async (context, next) =>
            {
                string token = context.Request.Headers["Authorization"];
                if (!string.IsNullOrWhiteSpace(token) && (token = token.Split(' ').Last()).Length == 32)
                {
                    var config = (IConfiguration)AppDomain.CurrentDomain.GetData("Configuration");
                    context.Request.Headers["Authorization"] = token;

                    context.Request.Headers["Authorization"] = "Bearer " + JwtToken.GenerateToken(config, token);
                }

                await next.Invoke();
            });
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalHub>("/signalHub");
            });

            app.UseMvc();

            AppDomain.CurrentDomain.SetData("DataPath", Path.GetFullPath(env.ContentRootPath + $"{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}"));
            AppDomain.CurrentDomain.SetData("Configuration", Configuration);
            AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
            AppDomain.CurrentDomain.SetData("Assemblies", Configuration.GetValue<String>("Assemblies").Split(','));

            // load dll dynamic
            /*Assembly library = Assembly.LoadFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Quickflow.ActivityRepository.dll"));
            Type myClass = (from type in library.GetExportedTypes()
                where typeof(IMyInterface).IsAssignableFrom(type)
                select type)
                .Single();*/

            InitializationLoader loader = new InitializationLoader();
            loader.Env = env;
            loader.Config = Configuration;
            loader.Load();
        }
    }
}
