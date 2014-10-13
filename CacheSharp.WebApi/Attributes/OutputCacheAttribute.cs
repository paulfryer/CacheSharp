using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CacheSharp.WebApi.Attributes
{
    public class OutputCacheAttribute : ActionFilterAttribute
    {
        public const int DefaultTimeoutMilliseconds = 10*1000;
        public const bool DefaultCacheOnClient = false;
        public const string DefaultContentType = "text/json";
        private readonly IAsyncCache cache;
        private readonly bool cacheOnClient;
        private readonly string contentType;
        private readonly int timeoutMilliseconds;


        public OutputCacheAttribute(
            int timeoutMilliseconds = DefaultTimeoutMilliseconds,
            bool cacheOnClient = DefaultCacheOnClient,
            string contentType = DefaultContentType)
        {
            this.timeoutMilliseconds =
                ConfigurationManager.AppSettings.AllKeys.Contains("OutputCache.TimeoutMilliseconds")
                    ? int.Parse(ConfigurationManager.AppSettings["OutputCache.TimeoutMilliseconds"])
                    : timeoutMilliseconds;

            this.cacheOnClient = ConfigurationManager.AppSettings.AllKeys.Contains("OutputCache.CacheOnClient")
                ? bool.Parse(ConfigurationManager.AppSettings["OutputCache.CacheOnClient"])
                : cacheOnClient;

            this.contentType = ConfigurationManager.AppSettings.AllKeys.Contains("OutputCache.ContentType")
                ? ConfigurationManager.AppSettings["OutputCache.ContentType"]
                : contentType;

            cache = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof (IAsyncCache)) as IAsyncCache;
        }


        public override async Task OnActionExecutingAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            var cacheKey = actionContext.ActionArguments.ToCacheKey();
            var value = await cache.GetAsync<string>(cacheKey);
            if (value != null)
            {
                actionContext.Response = actionContext.Request.CreateResponse();
                if (cacheOnClient)
                    actionContext.Response.StatusCode = HttpStatusCode.NotModified;
                else
                {
                    actionContext.Response.Content = new StringContent(value);
                    actionContext.Response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
            }
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            var cacheKey = actionExecutedContext.ActionContext.ActionArguments.ToCacheKey();
            var content = await actionExecutedContext.Response.Content.ReadAsStringAsync();
            if (cacheOnClient)
            {
                var eTag = GetETag(cacheKey);
                actionExecutedContext.ActionContext.Response.Headers.ETag = new EntityTagHeaderValue(GetETag(cacheKey));
                await cache.PutAsync(cacheKey, eTag, TimeSpan.FromMilliseconds(timeoutMilliseconds));
            }
            else await cache.PutAsync(cacheKey, content, TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        private string GetETag(string content)
        {
            return string.Format("\"{0}\"", content.GetHashCode());
        }
    }
}