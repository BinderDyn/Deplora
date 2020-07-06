using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
