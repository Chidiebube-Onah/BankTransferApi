using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BankTransfer.BLL.Implementations;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RestSharp;

namespace BankTransfer.BLL.Extensions
{
    public static class RestClientHandlerExtension
    {
        private static AsyncRetryPolicy<IRestResponse>? _retryPolicy;
        public static Task<IRestResponse> RetryAsync(this Task<IRestResponse> task, int maxRetryAttempt)
        {
            if (maxRetryAttempt <= 0) return task;

            _retryPolicy = Policy.HandleResult<IRestResponse>(r => !r.IsSuccessful && ( r.ResponseStatus is ResponseStatus.TimedOut or ResponseStatus.Error || r.ErrorException!=null))
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(maxRetryAttempt, retryAttempt => {
                        var timeToWait = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                        Console.WriteLine($"retry connection in {timeToWait.TotalSeconds} seconds");
                        return timeToWait;
                    }
                );

            return _retryPolicy.ExecuteAsync( () => task);

        }
    }
}