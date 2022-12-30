using BankTransfer.Api.Handlers;
using BankTransfer.Api.Options;
using BankTransfer.BLL.Factories;
using BankTransfer.BLL.Implementations;
using BankTransfer.BLL.Interfaces;
using BankTransfer.Models.Configs;
using Microsoft.AspNetCore.Mvc;

namespace BankTransfer.Api.Extensions
{
    public static class ProgramExtension
    {
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services
                .AddScoped<IClientsService, InMemoryClientsService>()
                .AddScoped<IApiKeyCacheService, ApiKeyCacheService>()
                .AddScoped<ApiKeyAuthenticationHandler>()
                .AddScoped<BankTransferFactory>()
                .AddScoped<RestClientHandler>()
                .AddScoped<KudaProviderService>()
                .AddScoped<IBankTransferService, KudaProviderService>(s =>
                    s.GetService<KudaProviderService>() ?? throw new ArgumentNullException())
                .AddScoped<FlutterWaveProviderService>()
                .AddScoped<IBankTransferService, FlutterWaveProviderService>(s =>
                    s.GetService<FlutterWaveProviderService>() ?? throw new ArgumentNullException());

        }

        public static IServiceCollection BindConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            FlutterWaveConfig flutterWaveConfig = new FlutterWaveConfig();

            configuration.GetSection(nameof(flutterWaveConfig)).Bind(flutterWaveConfig);

            services.AddSingleton(flutterWaveConfig);


            KudaBankConfig kudaBankConfig = new KudaBankConfig();

            configuration.GetSection(nameof(kudaBankConfig)).Bind(kudaBankConfig);

            services.AddSingleton(kudaBankConfig);

            return services;
        }
    }
}