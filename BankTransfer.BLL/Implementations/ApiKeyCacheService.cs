using BankTransfer.BLL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BankTransfer.BLL.Implementations
{
    public class ApiKeyCacheService : IApiKeyCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IClientsService _clientsService;

        public ApiKeyCacheService(IMemoryCache memoryCache, IClientsService clientsService)
        {
            _memoryCache = memoryCache;
            _clientsService = clientsService;
        }


        private const string Prefix = "BT-";
        public async Task<string> GenerateApiKey()
        {
            using RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[32];
            provider.GetBytes(bytes);


            string key = Prefix + Convert.ToBase64String(bytes)
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "")[..33];


            await InvalidateAllApiKeys();

            await _clientsService.Add(key);

            return key;
        }


        public async ValueTask<Guid> GetClientIdFromApiKey(string apiKey)
        {
            if (!_memoryCache.TryGetValue<Dictionary<string, Guid>>("Authentication_ApiKeys", out Dictionary<string, Guid>? internalKeys))
            {
                internalKeys = await _clientsService.GetActiveClients();

                _memoryCache.Set("Authentication_ApiKeys", internalKeys);
            }

            if (!internalKeys.TryGetValue(apiKey, out Guid clientId))
            {
                return Guid.Empty;
            }

            return clientId;
        }

        public async Task InvalidateApiKey(string apiKey)
        {
            if (_memoryCache.TryGetValue<Dictionary<string, Guid>>("Authentication_ApiKeys", out Dictionary<string, Guid>? internalKeys))
            {
                if (internalKeys.ContainsKey(apiKey))
                {
                    internalKeys.Remove(apiKey);
                    _memoryCache.Set("Authentication_ApiKeys", internalKeys);
                }
            }

            await _clientsService.InvalidateApiKey(apiKey);
        }


        private Task InvalidateAllApiKeys()
        {
            if (_memoryCache.TryGetValue<Dictionary<string, Guid>>("Authentication_ApiKeys", out Dictionary<string, Guid>? internalKeys))
            {
                _memoryCache.Remove("Authentication_ApiKeys");
            }

            return Task.CompletedTask;
        }
    }

}