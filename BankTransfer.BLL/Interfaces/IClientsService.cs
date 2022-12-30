using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankTransfer.BLL.Interfaces
{
    public interface IClientsService
    {
        Task Add(string key);
        Task<Dictionary<string, Guid>> GetActiveClients();
        Task InvalidateApiKey(string apiKey);
    }
}