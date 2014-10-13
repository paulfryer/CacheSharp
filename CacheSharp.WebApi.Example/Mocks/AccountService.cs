using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CacheSharp.WebApi.Example.Mocks
{
    public class AccountService : IAccountService
    {
        private static readonly Dictionary<string, List<Transfer>> TransferDatabase = new Dictionary<string, List<Transfer>>();

        public AccountService()
        {
            var random = new Random();
            // Generate 10 users
            for (var i = 1000; i < 1010; i++)
            {
                var userId = i.ToString();
                //var userAccounts = GetAccounts(userId).ToList();

                // generate a 1000 transfers
                for (var t = 0; t < 1000; t++)
                {
                    var fromAccountId = random.Next(10).ToString();
                    var toAccountId = random.Next(10).ToString();
                    var amount = random.Next(10000);
                    Transfer(userId, fromAccountId, toAccountId, amount);
                }
            }
        }

        public IEnumerable<Account> GetAccounts(string userId)
        {
            SimulateProcessingTime();

            for (var i = 0; i > 10; i++)
                yield return new Account
                {
                    Id = i.ToString(),
                    Number = userId + "000" + i,
                    Balance = i + 100*2,
                    Type = "Checking"
                };
        }

        public string Transfer(string userId, string fromAccountId, string toAccountId, decimal amount)
        {
            var transfer = new Transfer
            {
                Amount = amount,
                Date = DateTime.UtcNow,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Id = Guid.NewGuid().ToString()
            };
            if (!TransferDatabase.ContainsKey(userId))
                TransferDatabase.Add(userId, new List<Transfer>());
            TransferDatabase[userId].Add(transfer);
            return transfer.Id;
        }

        public IEnumerable<Transfer> GetTransferHistory(string userId, string accountId)
        {
            SimulateProcessingTime();
            return TransferDatabase[userId].Where(tran => tran.FromAccountId == accountId);
        }

        private void SimulateProcessingTime()
        {
            var random = new Random();
            var processingTime = random.Next(1000, 10000);
            Thread.Sleep(processingTime);
        }
    }
}