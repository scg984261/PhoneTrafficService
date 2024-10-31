using PhoneTrafficService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhoneTrafficServiceTest
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void TestGetIncomingFilePathFromConfig_ShouldReturnValue()
        {
            string testConfigValue = Program.GetIncomingFilePathFromConfig();
            Assert.AreEqual("test value 123", testConfigValue);
        }
    }
}
