using BankTransfer.Models.Dtos.Responses;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BankTransfer.Api.Handlers;

public static class ExceptionHandler
{
    public static void ConfigureException(this IApplicationBuilder app, IWebHostEnvironment hostEnvironment)
    {

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                IExceptionHandlerFeature? exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandleFeature != null)
                {
                    switch (exceptionHandleFeature.Error)
                    {
                        case InvalidOperationException:
                        case ArgumentException:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            break;
                        default:
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            break;
                    }

                    ErrorResponse err = new()
                    {
                        Code = context.Response.StatusCode.ToString(),
                        Description = hostEnvironment.IsProduction() && context.Response.StatusCode ==
                            StatusCodes.Status500InternalServerError
                                ? "We currently cannot complete this request process. Please retry or contact our support team !"
                                : exceptionHandleFeature.Error.Message
                    };

                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                    serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    string msg = JsonConvert.SerializeObject(err, serializerSettings);
                    await context.Response.WriteAsync(msg);
                }
            });
        });
    }
}