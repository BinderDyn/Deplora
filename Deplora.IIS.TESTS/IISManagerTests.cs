using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Deplora.IIS.TESTS
{
    [TestClass]
    public class IISManagerTests
    {
        // NOTE: This tests will only work if you have IIS/IIS Express installed

        [TestMethod]
        public void ExecuteCommand_Test()
        {
            // ARRANGE
            string webSiteName = "Default Web Site";
            string appPoolName = "DefaultAppPool";
            string iisPath = "C:\\Windows\\System32\\inetsrv";
            var iisManager = new IISManager(appPoolName, iisPath, webSiteName);

            // ACT
            bool success = iisManager.ExecuteCommand("echo hello");

            // ASSERT
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void StartWebsite_Test()
        {
            // ARRANGE
            string webSiteName = "Default Web Site";
            string appPoolName = "DefaultAppPool";
            string iisPath = "C:\\Windows\\System32\\inetsrv\\";
            var iisManager = new IISManager(appPoolName, iisPath, webSiteName);

            // ACT
            iisManager.StartWebsite();
            Thread.Sleep(1000);
        }

        [TestMethod]
        public void StopWebsite_Test()
        {
            // ARRANGE
            string webSiteName = "Default Web Site";
            string appPoolName = "DefaultAppPool";
            string iisPath = "C:\\Windows\\System32\\inetsrv\\";
            var iisManager = new IISManager(appPoolName, iisPath, webSiteName);

            // ACT
            iisManager.StopWebsite();
            Thread.Sleep(1000);
        }

        [TestMethod]
        public void StartAppPool_Test()
        {
            // ARRANGE
            string webSiteName = "Default Web Site";
            string appPoolName = "DefaultAppPool";
            string iisPath = "C:\\Windows\\System32\\inetsrv\\";
            var iisManager = new IISManager(appPoolName, iisPath, webSiteName);

            // ACT
            iisManager.StartAppPool();
            Thread.Sleep(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void StartAppPool_Test_WrongName()
        {
            // ARRANGE
            string webSiteName = "Default Web Site";
            string appPoolName = "DefaultAppPoolX";
            string iisPath = "C:\\Windows\\System32\\inetsrv\\";
            var iisManager = new IISManager(appPoolName, iisPath, webSiteName);

            // ACT
            iisManager.StartAppPool();
            Thread.Sleep(1000);
        }

        [TestMethod]
        public void StopAppPool_Test()
        {
            // ARRANGE
            string webSiteName = "Default Web Site";
            string appPoolName = "DefaultAppPool";
            string iisPath = "C:\\Windows\\System32\\inetsrv\\";
            var iisManager = new IISManager(appPoolName, iisPath, webSiteName);

            // ACT
            iisManager.StopAppPool();
            Thread.Sleep(1000);
        }
    }
}
