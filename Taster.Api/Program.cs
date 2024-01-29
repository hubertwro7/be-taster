using Serilog;
using Taster.Api.Middlewares;
using Taster.Application.Logic.Abstractions;
using Taster.Infrastructure.Persistence;
using Taster.Application;
using Taster.Infrastructure.Auth;
using Taster.Api.Application.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Taster.Api
{
    public class Program
    {
        public static string APP_NAME = "Taster.Api";
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Application", APP_NAME)
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddJsonFile("appsettings.Development.local.json");
            }

            builder.Host.UseSerilog((context, services, configuration) => configuration
                .Enrich.WithProperty("Application", APP_NAME)
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDatabaseCache();
            builder.Services.AddSqlDatabase(builder.Configuration.GetConnectionString("MainDbSql")!);
            builder.Services.AddControllersWithViews(options =>
            {
                if(!builder.Environment.IsDevelopment())
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                }
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddJwtAuth(builder.Configuration);
            builder.Services.AddJwtAuthenticationDataProvider(builder.Configuration);
            builder.Services.AddPasswordManager();

            builder.Services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblyContaining(typeof(BaseCommandHandler));
            });

            builder.Services.AddApplicationServices();
            builder.Services.AddValidators();

            builder.Services.AddSwaggerGen(x =>
            {
                x.CustomSchemaIds(y =>
                {
                    var name = y.FullName;
                    if (name != null)
                    {
                        name = name.Replace("+", "_");
                    }

                    return name;
                });
            });

            builder.Services.AddAntiforgery(x =>
            {
                x.HeaderName = "X-XSRF-TOKEN";
            });

            builder.Services.AddCors();

            var app = builder.Build();

            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(builder => builder
            .WithOrigins(app.Configuration.GetValue<string>("WebAppBaseUrl") ?? "")
            .WithOrigins(app.Configuration.GetSection("AdditionalCorsOrigins").Get<string[]>() ?? new string[0])
            .WithOrigins((Environment.GetEnvironmentVariable("AdditionalCorsOrigins") ?? "").Split(",").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).ToArray())
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod());

            app.UseExceptionResultMiddleware();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
