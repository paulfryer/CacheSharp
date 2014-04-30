using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CacheSharp.SQLServer
{
    public sealed class SqlAsyncCache : IAsyncCache<string>, IInitializable, IDisposable
    {
        private DbConnection conn;

        public async Task PutAsync(string key, string value, TimeSpan lifeSpan)
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

        public async Task<string> GetAsync(string key)
        {
            try
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
                string value = outValue.SqlValue.ToString();
                return value;
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number == 41301)
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
                    getCommand.ExecuteNonQuery();
                    string value = outValue.SqlValue.ToString();
                    return value;
                }
                throw;
            }
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