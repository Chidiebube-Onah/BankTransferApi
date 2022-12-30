using System;
using System.Threading.Tasks;

namespace BankTransfer.BLL.Interfaces
{
    public interface IApiKeyCacheService
    {
        Task<string> GenerateApiKey();
        ValueTask<Guid> GetClientIdFromApiKey(string apiKey);
        Task InvalidateApiKey(string apiKey);
    }
}