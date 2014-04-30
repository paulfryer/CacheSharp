using System;
using System.Configuration;

namespace CacheSharp.Console
{
    public class FileLoadConfiguration : ILoadConfiguration
    {
        public int ParallelInstances
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["ParallelInstances"]); }
        }

        public int IterationsPerInstance
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["IterationsPerInstance"]); }
        }

        public string CacheProvider
        {
            get { return ConfigurationManager.AppSettings["CacheProvider"]; }
        }

        public int CharactersPerMessage
        {

            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CharactersPerMessage"]); 
            }
            

        }
    }
}