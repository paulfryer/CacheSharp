using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CacheSharp.WebApi.Attributes
{
    public class ThrottleAttribute : AuthorizationFilterAttribute
    {
        private readonly IAsyncCache cache =
            GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAsyncCache)) as IAsyncCache;

        private readonly int maxRequests;
        private readonly int windowSeconds;

        public ThrottleAttribute()
        {
            maxRequests = ConfigurationManager.AppSettings.AllKeys.Contains("Throttle.MaxRequests")
                ? int.Parse(ConfigurationManager.AppSettings["Throttle.MaxRequests"])
                : DefaultMaxRequests;
            windowSeconds = ConfigurationManager.AppSettings.AllKeys.Contains("Throttle.WindowSeconds")
                ? int.Parse(ConfigurationManager.AppSettings["Throttle.WindowSeconds"])
                : DefaultWindowSeconds;
        }

        public ThrottleAttribute(int maxRequests, int windowSeconds)
        {
            this.maxRequests = maxRequests;
            this.windowSeconds = windowSeconds;
        }

        public int DefaultMaxRequests
        {
            get { return 10; }
        }

        public int DefaultWindowSeconds
        {
            get { return 1; }
        }


        public TimeSpan Window
        {
            get { return TimeSpan.FromSeconds(windowSeconds); }
        }

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            var ipAddress = HttpContext.Current.Request.UserHostAddress;
            var cacheKey = string.Format("{0}.{1}.RequestLog", CacheScope.User, ipAddress);
            var requestLog = await cache.GetAsync<List<DateTime>>(cacheKey) ?? new List<DateTime>();

            if (requestLog.Count > maxRequests)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase =
                        "Too many requests! Max allowed requests is " + maxRequests + " requests per " + windowSeconds +
                        " seconds."
                });

            requestLog.Add(DateTime.UtcNow);
            requestLog.RemoveAll(time => time < DateTime.UtcNow.Subtract(Window));
            await cache.PutAsync(cacheKey, requestLog, Window);
        }


        public class RequestProfile
        {
            public int Count { get; set; }
            public DateTime Since { get; set; }
        }
    }
}
