using System;

namespace CacheSharp.WebApi.Example.Mocks
{
    public class Transfer
    {
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Id { get; set; }
    }
}