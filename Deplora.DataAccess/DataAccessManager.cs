using Deplora.Shared.Enums;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
        private async Task<SqlCommandExecutionResult> ExecuteMsSqlCommands(string commands, bool useTransaction)
        {
            var result = new SqlCommandExecutionResult();
            if (useTransaction) await TryMsSqlTransaction(commands, result);
            else await ExecuteMsSqlWithoutTransaction(commands, result);
            return result;
        }

        /// <summary>
        /// Executes MSSQL commands without a transaction scope
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ExecuteMsSqlWithoutTransaction(string commands, SqlCommandExecutionResult result)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = connection.CreateCommand();
                    command.Connection = connection;
                    command.CommandText = commands;
                    await connection.OpenAsync();
                    var queryResult = await command.ExecuteNonQueryAsync();
                    result.Message = GetRowsAffected(queryResult);
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                    result.Message = ex.Message;
                    result.Success = false;
                }
            }
        }

        /// <summary>
        /// Executes MSSQL commands within a transaction scope and will try rollback on fail
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task TryMsSqlTransaction(string commands, SqlCommandExecutionResult result)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("ExecutingSqlCommands");
                    command.Connection = connection;
                    command.Transaction = transaction;
                    try
                    {
                        command.CommandText = commands;
                        var queryResult = await command.ExecuteNonQueryAsync();
                        await transaction.CommitAsync();
                        result.Success = true;
                        result.Message = GetRowsAffected(queryResult);
                    }
                    catch (Exception commitException)
                    {
                        result.Exception = commitException;
                        result.Message = string.Format("{0}{1}{2}", commitException.Message, System.Environment.NewLine, "Rolling back transaction");
                        result.Success = false;
                        try
                        {
                            await transaction.RollbackAsync();
                        }
                        catch (Exception rollbackException)
                        {
                            result.Exception = rollbackException;
                            result.Message = rollbackException.Message;
                            result.Success = false;
                        }
                    }
                }
                catch (Exception connectionException)
                {
                    result.Exception = connectionException;
                    result.Message = connectionException.Message;
                    result.Success = false;
                }
            }
        }

        /// <summary>
        /// Executes SQL commands on a MySql-Adapter
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        private async Task<SqlCommandExecutionResult> ExecuteMySqlCommands(string commands, bool useTransaction)
        {
            var result = new SqlCommandExecutionResult();
            if (useTransaction) await TryMySqlTransaction(commands, result);
            else await ExecuteMySqlCommandsWithoutTransaction(commands, result);
            return result;
        }

        /// <summary>
        /// Executes MySql commands without a transaction scope
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ExecuteMySqlCommandsWithoutTransaction(string commands, SqlCommandExecutionResult result)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.Connection = connection;
                    command.CommandText = commands;
                    await connection.OpenAsync();
                    var queryResult = await command.ExecuteNonQueryAsync();
                    result.Message = GetRowsAffected(queryResult);
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                    result.Message = ex.Message;
                    result.Success = false;
                }
            }
        }

        /// <summary>
        /// Executes MySql commands within a transaction scope and will try rollback on fail
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task TryMySqlTransaction(string commands, SqlCommandExecutionResult result)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    MySqlCommand command = connection.CreateCommand();
                    MySqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    try
                    {
                        command.CommandText = commands;
                        var queryResult = await command.ExecuteNonQueryAsync();
                        await transaction.CommitAsync();
                        result.Success = true;
                        result.Message = GetRowsAffected(queryResult);
                    }
                    catch (Exception commitException)
                    {
                        result.Exception = commitException;
                        result.Message = string.Format("{0}{1}{2}", commitException.Message, System.Environment.NewLine, "Rolling back transaction");
                        result.Success = false;
                        try
                        {
                            await transaction.RollbackAsync();
                        }
                        catch (Exception rollbackException)
                        {
                            result.Exception = rollbackException;
                            result.Message = rollbackException.Message;
                            result.Success = false;
                        }
                    }
                }
                catch (Exception connectionException)
                {
                    result.Exception = connectionException;
                    result.Message = connectionException.Message;
                    result.Success = false;
                }
            }
        }

        /// <summary>
        /// Executes SQL commands based on the given database adapter. Returns an empty result if no database adapter provided in class
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        public async Task<SqlCommandExecutionResult> ExecuteSqlCommands(string commands, bool useTransaction = true)
        {
            return databaseAdapter switch
            {
                DatabaseAdapter.None => new SqlCommandExecutionResult() { Success = false, Message = "No database adapter specified" },
                DatabaseAdapter.MSSQL => await ExecuteMsSqlCommands(commands, useTransaction),
                DatabaseAdapter.MySQL => await ExecuteMySqlCommands(commands, useTransaction),
                _ => throw new NotImplementedException(databaseAdapter.ToString())
            };
        }

        /// <summary>
        /// Creates a backup of the database in the specified folder
        /// </summary>
        /// <param name="backupPath"></param>
        /// <returns></returns>
        public async Task<SqlCommandExecutionResult> BackupDatabase(string backupPath, bool useTransaction = false)
        {
            var command = string.Format("BACKUP DATABASE {0} TO DISK = '{1}';", GetDatabaseName(connectionString), backupPath);
            return await ExecuteSqlCommands(command, useTransaction);
        }

        /// <summary>
        /// Gets the initial catalog of a given connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static string GetDatabaseName(string connectionString)
        {
            var databaseName = string.Empty;
            if (connectionString.Contains("Initial catalog="))
                databaseName = GetConnectionStringValue("Initial catalog", connectionString);
            else if (connectionString.Contains("Database="))
                databaseName = GetConnectionStringValue("Database", connectionString);
            return databaseName;
        }

        /// <summary>
        /// Gets part of the connection string value by its identifier
        /// </summary>
        /// <param name="separationSegment">E.g. "Database=", "Initial catalog="</param>
        /// <returns></returns>
        private static string GetConnectionStringValue(string separationSegment, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(separationSegment)) throw new ArgumentNullException("separationSegment");
            if (separationSegment.LastOrDefault() != '=') separationSegment += "=";
            var twoPartConnectionString = connectionString.Split(separationSegment);
            var initialCatalog = twoPartConnectionString[1].Split(";");
            return initialCatalog[0];
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
