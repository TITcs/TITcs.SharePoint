using NUnit.Framework;
using TITcs.SharePoint.Utils;

namespace TITcs.SharePoint.Test.Utils
{
    [TestFixture]
    public class AppSettingsUtilsTest
    {
        [Test]
        public void ReadKeys()
        {
            Assert.IsTrue(AppSettingsUtils.UserName == "User");

            Assert.IsTrue(AppSettingsUtils.Password == "P@ssw0rd");

            Assert.IsTrue(AppSettingsUtils.NetDomain == "DOMAIN");

            Assert.IsTrue(AppSettingsUtils.Root == "http://titcs.sharepoint");
        }
    }
}
