using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankTransfer.BLL.Interfaces;

namespace BankTransfer.BLL.Implementations
{
    public class InMemoryClientsService : IClientsService
    {
        private static Dictionary<string, Guid> _clients = new()
        {
            { "BT-mTbC4r1Eh7wvXrXE1UDl18NGH1fRzcrRz", Guid.NewGuid() }
        };

        public Task Add(string key)
        {
            _clients.Clear();
            _clients.Add(key, Guid.NewGuid());

            return Task.CompletedTask;
        }

        public Task<Dictionary<string, Guid>> GetActiveClients()
        {
            return Task.FromResult(_clients);
        }

        public Task InvalidateApiKey(string apiKey)
        {
            _clients.Remove(apiKey);

            return Task.CompletedTask;
        }
    }
}