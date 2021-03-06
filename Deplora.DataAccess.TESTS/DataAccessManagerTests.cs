﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Deplora.DataAccess.TESTS
{
    [TestClass]
    public class DataAccessManagerTests
    {
        private string defaultConnectionString { get => @"Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=true;"; }
        private string connectionStringInitialCatalog { get => @"Server=(localdb)\MSSQLLocalDB;Initial catalog=master;Integrated Security=true;"; }
        private string initialPath { get => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }

        [TestMethod]
        public void GetDatabaseName_Test_Database()
        {
            // ARRANGE
            // ...

            // ACT
            var databaseName = DataAccessManager.GetDatabaseName(defaultConnectionString);

            // ASSERT
            Assert.AreEqual("master", databaseName);
        }

        [TestMethod]
        public void GetDatabaseName_Test_InitialCatalog()
        {
            // ARRANGE
            // ...

            // ACT
            var databaseName = DataAccessManager.GetDatabaseName(connectionStringInitialCatalog);

            // ASSERT
            Assert.AreEqual("master", databaseName);
        }

        [TestMethod]
        public void BackupDatabase_Test_MSSQL()
        {
            // ARRANGE
            var dataAccessManager = new DataAccessManager(defaultConnectionString, Shared.Enums.DatabaseAdapter.MSSQL);
            var backupFilePath = Path.Combine(initialPath, string.Format("{0:yyyyMMdd}_BACKUP.bak", DateTime.Now));
            // ACT
            var result = Task.Run(() => dataAccessManager.BackupDatabase(backupFilePath)).GetAwaiter().GetResult();

            // ASSERT
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(File.Exists(backupFilePath));
        }

        [TestMethod]
        public void CanEstablishConnection_Test_MSSQL()
        {
            // ARRANGE
            var dataAccessManager = new DataAccessManager(defaultConnectionString, Shared.Enums.DatabaseAdapter.MSSQL);

            // ACT
            var result = Task.Run(() => dataAccessManager.CanEstablishConnection()).GetAwaiter().GetResult();

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanEstablishConnection_Test_MSSQL_WrongConnectionString()
        {
            // ARRANGE
            var falseConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=xyz;Integrated Security=true;";
            var dataAccessManager = new DataAccessManager(falseConnectionString, Shared.Enums.DatabaseAdapter.MSSQL);

            // ACT
            var result = Task.Run(() => dataAccessManager.CanEstablishConnection()).GetAwaiter().GetResult();

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanEstablishConnection_Test_MSSQL_WrongAdapter()
        {
            // ARRANGE
            var falseConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=xyz;Integrated Security=true;";
            var dataAccessManager = new DataAccessManager(falseConnectionString, Shared.Enums.DatabaseAdapter.MySQL);

            // ACT
            var result = Task.Run(() => dataAccessManager.CanEstablishConnection()).GetAwaiter().GetResult();

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExecuteSqlCommands_Test_MSSQL()
        {
            // ARRANGE
            var sqlCommands = "SELECT name FROM master.dbo.sysdatabases";
            var dataAccessManager = new DataAccessManager(defaultConnectionString, Shared.Enums.DatabaseAdapter.MSSQL);

            // ACT
            var result = Task.Run(() => dataAccessManager.ExecuteSqlCommands(sqlCommands)).GetAwaiter().GetResult();

            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual("-1 rows affected", result.Message);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void ExecuteSqlCommands_Test_MSSQL_MultipleRows()
        {
            // ARRANGE
            var sqlCommands = @"SELECT * FROM spt_fallback_db
                                SELECT * FROM MSreplication_options";
            var dataAccessManager = new DataAccessManager(defaultConnectionString, Shared.Enums.DatabaseAdapter.MSSQL);

            // ACT
            var result = Task.Run(() => dataAccessManager.ExecuteSqlCommands(sqlCommands)).GetAwaiter().GetResult();

            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual("-1 rows affected", result.Message);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void ExecuteSqlCommands_Test_NoAdapter()
        {
            // ARRANGE
            var sqlCommands = "SELECT * FROM ApplicationUsers";
            var dataAccessManager = new DataAccessManager(defaultConnectionString, Shared.Enums.DatabaseAdapter.None);

            // ACT
            var result = Task.Run(() => dataAccessManager.ExecuteSqlCommands(sqlCommands)).GetAwaiter().GetResult();

            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual("No database adapter specified", result.Message);
            Assert.IsFalse(result.Success);
        }
    }
}
