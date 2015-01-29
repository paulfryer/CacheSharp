using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using CacheSharp.Redis;
using CacheSharp.WebApi.Example.Controllers;
using CacheSharp.WebApi.Example.Mocks;

namespace CacheSharp.WebApi.Example
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ConfigureDependencies();

           GlobalConfiguration.Configure(WebApiConfig.Register);


        }

        private static void ConfigureDependencies()
        {
            var cache = new RedisCache();
            cache.InitializeAsync(new Dictionary<string, string>
            {
                {"Endpoint", ConfigurationManager.AppSettings["Redis.Endpoint"]},
                {"Key", ConfigurationManager.AppSettings["Redis.Key"]},
                {"UseSsl", ConfigurationManager.AppSettings["Redis.UseSsl"]}
            });


            var builder = new ContainerBuilder();
            builder.RegisterType<AccountService>().As<IAccountService>().SingleInstance();
            builder.RegisterType<TransfersController>();

            builder.RegisterInstance(cache).As<IAsyncCache>().SingleInstance();
            builder.RegisterInstance(cache).As<ISyncCache>().SingleInstance();

            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
           
        }
    }
}
