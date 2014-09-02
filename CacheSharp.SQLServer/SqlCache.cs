using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CacheSharp.SQLServer
{
    public sealed class SqlCache : IAsyncCache, IInitializable, IDisposable
    {
        private DbConnection conn;

        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "PutAppState";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@KeyID", key));
                command.Parameters.Add(new SqlParameter("@value", value));
                command.Parameters.Add(new SqlParameter("@TimeoutUTC", DateTime.UtcNow.AddMinutes(5)));
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number == 41301)
                {
                    // TODO: think about a recursive try/get pattern here (to some max iterations setting).
                    DbCommand command = conn.CreateCommand();
                    command.CommandText = "PutAppState";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@KeyID", key));
                    command.Parameters.Add(new SqlParameter("@value", value));
                    command.Parameters.Add(new SqlParameter("@TimeoutUTC", DateTime.UtcNow.AddMinutes(5)));
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            
                DbCommand getCommand = conn.CreateCommand();
                getCommand.CommandText = "GetAppState";
                getCommand.Parameters.Add(new SqlParameter("@KeyID", key));
                var outValue = new SqlParameter
                {
                    ParameterName = "@Value",
                    Direction = ParameterDirection.Output,
                    Size = 10000
                };
                getCommand.Parameters.Add(outValue);
                getCommand.CommandType = CommandType.StoredProcedure;
                await getCommand.ExecuteNonQueryAsync();
                string json = outValue.SqlValue.ToString();
                var value = JsonConvert.DeserializeObject<T>(json);
                return value;
            
            
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                DbCommand command = conn.CreateCommand();
                command.CommandText = "DeleteAppState";
                command.Parameters.Add(new SqlParameter("@KeyID", key));
                command.CommandType = CommandType.StoredProcedure;
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number == 41301)
                {
                    DbCommand command = conn.CreateCommand();
                    command.CommandText = "DeleteAppState";
                    command.Parameters.Add(new SqlParameter("@KeyID", key));
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            conn.Close();
            conn.Dispose();
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            string connString = parameters["ConnectionString"];
            conn = new SqlConnection(connString);
            await conn.OpenAsync();
        }

        public List<string> InitializationProperties
        {
            get { return new List<string> {"ConnectionString", "CharactersPerMessage"}; }
        }

        public string ProviderName
        {
            get { return "Sql"; }
        }
    }
}