using System.Collections.Generic;

namespace CacheSharp.WebApi.Example.Mocks
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccounts(string userId);
        string Transfer(string userId, string fromAccountId, string toAccountId, decimal amount);
        IEnumerable<Transfer> GetTransferHistory(string userId, string accountId);
    }
}