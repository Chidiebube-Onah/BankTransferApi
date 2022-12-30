using BankTransfer.Models.Dtos.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Net;

namespace BankTransfer.BLL.Extensions
{
    public static class ProviderErrorResponseHandler
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static void HandleAnyErrors(this IRestResponse providerResponse, bool validateJsonFormat = true)
        {
            switch (providerResponse.IsSuccessful)
            {
                case true when validateJsonFormat:
                    {
                        var content = providerResponse.Content;
                        try
                        {
                            JObject.Parse(content);
                        }
                        catch (JsonReaderException e)
                        {
                            throw new InvalidOperationException(content);
                        }

                        break;
                    }
                case false:
                    {
                        switch (providerResponse.StatusCode)
                        {
                            case HttpStatusCode.Forbidden:
                            case HttpStatusCode.NotFound:
                            case HttpStatusCode.GatewayTimeout:
                            case HttpStatusCode.InternalServerError:
                            case HttpStatusCode.RequestTimeout:

                                throw new InvalidOperationException(
                                    $"provider responded with a status of '{providerResponse.StatusDescription}'");

                            default:

                                if (providerResponse.ErrorException != null)
                                {
                                    throw new InvalidOperationException(providerResponse.ErrorException.Message);
                                }

                                string? errorMsg = providerResponse.Content;
                                try
                                {
                                    JObject.Parse(errorMsg);
                                    ProviderErrorResponse? error =
                                        JsonConvert.DeserializeObject<ProviderErrorResponse>(errorMsg,
                                            JsonSerializerSettings);
                                    throw new InvalidOperationException(error?.Message);
                                }
                                catch (JsonReaderException e)
                                {
                                    try
                                    {
                                        errorMsg = JsonConvert.DeserializeObject<string>(errorMsg);
                                        throw new InvalidOperationException(errorMsg);

                                    }
                                    catch (JsonReaderException ex)
                                    {
                                        throw new InvalidOperationException(errorMsg);
                                    }

                                }
                        }
                    }
            }
        }
    }
}
