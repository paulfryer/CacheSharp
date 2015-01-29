using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using CacheSharp.WebApi.Attributes;
using CacheSharp.WebApi.Example.Mocks;

namespace CacheSharp.WebApi.Example.Controllers
{
    public class TransfersController : ApiController
    {
        private readonly IAccountService accountService;
        private readonly IAsyncCache cache;

        public TransfersController(IAccountService accountService, IAsyncCache cache)
        {
            this.accountService = accountService;
            this.cache = cache;
        }

        [Throttle]
        [OutputCache]
        [Route("api/transfers/{userId}/{accountId}")]
        public async Task<dynamic> Get(string userId, string accountId, int page = 1, int pageSize = 10)
        {
            return accountService
                .GetTransferHistory(userId, accountId)
                .Skip((page - 1)*pageSize).Take(pageSize)
                .ToList();
        }
    }
}