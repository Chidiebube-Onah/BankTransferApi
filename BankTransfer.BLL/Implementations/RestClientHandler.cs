using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankTransfer.BLL.Implementations
{
    public class RestClientHandler
    {
        private RestClient _client;

        public RestClient InstantiateClient(string baseUri, IAuthenticator? authenticator = null)
        {
            _client = null ?? new RestClient(baseUri);

            if (authenticator is not null)
            {
                _client.Authenticator = authenticator;
            }

            return _client;
        }

        public async Task<IRestResponse> PostAsync(string resource, string body)
        {
            RestRequest request = new RestRequest(resource);

            request.AddJsonBody(body);

            return await _client.ExecutePostAsync(request);
        }

        public async Task<IRestResponse> GetAsync(string resource, IEnumerable<Parameter>? parameters = null)
        {
            RestRequest request = new RestRequest(resource);

            if (parameters != null)
            {
                request.Parameters.AddRange(parameters);
            }

            return await _client.ExecuteGetAsync(request);
        }

    }
}