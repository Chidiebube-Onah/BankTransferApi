using BankTransfer.Api.Extensions;
using BankTransfer.Api.Handlers;
using BankTransfer.Api.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using BankTransfer.Models.Configs;

namespace BankTransfer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            string root = Directory.GetCurrentDirectory();
            string dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);


        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.RegisterServices();

            builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, null);

            //Add automapper
            builder.Services.AddAutoMapper(Assembly.Load("BankTransfer.BLL"));

            builder.Services.AddControllers();

            builder.Services.ConfigureVersioning();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(setup =>
            {
                setup.EnableAnnotations();
                setup.AddSecurityDefinition(ApiKeyAuthenticationOptions.DefaultScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = ApiKeyAuthenticationOptions.HeaderName,
                    Type = SecuritySchemeType.ApiKey
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = ApiKeyAuthenticationOptions.DefaultScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.BindConfigs(builder.Configuration);

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.ConfigureException(builder.Environment);

            app.UseHttpsRedirection();
            app.UseAuthentication();
            
            app.UseAuthorization();


            app.MapControllers();

            builder.Configuration
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{app.Environment.EnvironmentName}.json")
                .AddEnvironmentVariables();

            app.Run();
        }
    }
}