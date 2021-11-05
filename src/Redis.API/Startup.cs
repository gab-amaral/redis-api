using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Redis.Domain.Configuration;

namespace Redis.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationManager = configuration.GetSection("ConfigurationManager").Get<ConfigurationManager>();
        }

        public IConfiguration Configuration { get; }
        public ConfigurationManager ConfigurationManager { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => ConfigurationManager);

            services.AddSingleton(_ => StackExchange.Redis.ConnectionMultiplexer.Connect(ConfigurationManager.RedisConfiguration.ConnectionString));

            services.Scan(action =>
            {
                action.FromAssembliesOf(typeof(Services.ReferenceClass))
                      .AddClasses()
                      .AsImplementedInterfaces()
                      .WithScopedLifetime();
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Redis API", Version = "v1" });
            });

            services.AddRouting(opt => opt.LowercaseUrls = true);

            services.AddHealthChecks()
                    .AddRedis(ConfigurationManager.RedisConfiguration.ConnectionString,
                              name: "Redis",
                              timeout: System.TimeSpan.FromSeconds(3),
                              tags: new[] { "ready" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = async (context, report) =>
                    {
                        var result = new
                        {
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(entry => new
                            {
                                name = entry.Key,
                                status = entry.Value.Status.ToString(),
                                exception = entry.Value.Exception != null ? entry.Value.Exception.Message : string.Empty,
                                duration = entry.Value.Duration.ToString()
                            })
                        };

                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    }
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = (_) => false
                });
            });
        }
    }
}
