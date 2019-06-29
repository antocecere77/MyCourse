using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Options;

namespace MyCourse.Models.Services.Infrastructure
{
    public class SqlliteDatabaseAccessor : IDatabaseAccessor
    {

        private IOptionsMonitor<ConnectionStringsOptions> connectionStringsOptions;
        private readonly ILogger<SqlliteDatabaseAccessor> logger;

        public SqlliteDatabaseAccessor(ILogger<SqlliteDatabaseAccessor> logger, IOptionsMonitor<ConnectionStringsOptions> connectionStringsOptions)  
        {
            this.connectionStringsOptions = connectionStringsOptions;
            this.logger = logger;
        }

        public async Task<DataSet> QueryAsync(FormattableString formattableQuery)
        {
            logger.LogInformation(formattableQuery.Format, formattableQuery.GetArguments());

            var queryArguments = formattableQuery.GetArguments();
            var sqliteParameters = new List<SqliteParameter>();
            for(var i = 0; i <queryArguments.Length; i++) {
                var parameter = new SqliteParameter(i.ToString(), queryArguments[i]);
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i;
            }

            string query = formattableQuery.ToString();

            //Metodo alternativo per recuperare la connection string
            string connectionString = connectionStringsOptions.CurrentValue.Default;

            using(var conn = new SqliteConnection("Data Source=Data/MyCourse.db"))
            {
                await conn.OpenAsync();
                using(var cmd = new SqliteCommand(query, conn)) 
                {
                    cmd.Parameters.AddRange(sqliteParameters);
                    using(var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dataSet = new DataSet();
                        dataSet.EnforceConstraints = false;

                        do
                        {
                            var dataTable = new DataTable();
                            dataSet.Tables.Add(dataTable);
                            dataTable.Load(reader);
                        } while(!reader.IsClosed);
                      
                        return dataSet;
                    }
                    
                }    
            }            
        }
    }
}