using Deplora.Shared.Enums;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Deplora.DataAccess
{
    public class DataAccessManager
    {
        private readonly string connectionString;
        private readonly DatabaseAdapter databaseAdapter;

        public DataAccessManager(string connectionString, DatabaseAdapter databaseAdapterToUse)
        {
            this.connectionString = connectionString;
            this.databaseAdapter = databaseAdapterToUse;
        }

        /// <summary>
        /// Checks if the connection can be established
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<bool> CanEstablishConnection()
        {
            return databaseAdapter switch
            {
                DatabaseAdapter.None => false,
                DatabaseAdapter.MSSQL => await CanEstablishMSSQLConnection(connectionString),
                DatabaseAdapter.MySQL => await CanEstablishMySqlConnection(connectionString),
                _ => throw new NotImplementedException(databaseAdapter.ToString())
            };
        }

        /// <summary>
        /// Checks connectivity to MySql-Database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<bool> CanEstablishMySqlConnection(string connectionString)
        {
            var canEstablishConnection = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    canEstablishConnection = true;
                }
                catch
                {
                    canEstablishConnection = false;
                }
            }
            return canEstablishConnection;
        }

        /// <summary>
        /// Checks connectivity to MSSQL-Database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<bool> CanEstablishMSSQLConnection(string connectionString)
        {
            var canEstablishConnection = false;
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    canEstablishConnection = true;
                }
                catch
                {
                    canEstablishConnection = false;
                }
            }
            return canEstablishConnection;
        }

        /// <summary>
        /// Executes SQL commands on a MsSql-Adapter
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        private async Task<SqlCommandExecutionResult> ExecuteMsSqlCommands(string commands)
        {
            var result = new SqlCommandExecutionResult();
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(commands, connection);
                    var queryResult = await command.ExecuteNonQueryAsync();
                    result.Success = true;
                    result.Message = GetRowsAffected(queryResult);
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                    result.Message = ex.Message;
                    result.Success = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Executes SQL commands on a MySql-Adapter
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        private async Task<SqlCommandExecutionResult> ExecuteMySqlCommands(string commands)
        {
            var result = new SqlCommandExecutionResult();
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    var command = new MySqlCommand(commands, connection);
                    var queryResult = await command.ExecuteNonQueryAsync();
                    result.Success = true;
                    result.Message = GetRowsAffected(queryResult);
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                    result.Message = ex.Message;
                    result.Success = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Executes SQL commands based on the given database adapter. Returns an empty result if no database adapter provided in class
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        public async Task<SqlCommandExecutionResult> ExecuteSqlCommands(string commands)
        {
            return databaseAdapter switch
            {
                DatabaseAdapter.None => new SqlCommandExecutionResult() { Success = true, Message = "No database adapter specified" },
                DatabaseAdapter.MSSQL => await ExecuteMsSqlCommands(commands),
                DatabaseAdapter.MySQL => await ExecuteMySqlCommands(commands),
                _ => throw new NotImplementedException(databaseAdapter.ToString())
            };
        }

        public class SqlCommandExecutionResult
        {
            public SqlCommandExecutionResult(Exception ex = null, string message = null)
            {
                this.Exception = ex;
                this.Message = message;
            }

            public Exception Exception { get; set; }
            public string Message { get; set; }
            public bool Success { get; set; }
        }

        /// <summary>
        /// Formats the number of rows into a string
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private string GetRowsAffected(int rows)
        {
            return string.Format("{0} rows affected", rows);
        }
    }
}
