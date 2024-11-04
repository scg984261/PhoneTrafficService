using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhoneTrafficService;
using System.IO;

namespace PhoneTrafficServiceTest
{
    [TestClass]
    public class SpreadsheetHandlerTest
    {
        [TestMethod]
        public void TestConstructor()
        {
            string filePath = @"Resources\Phone Numbers Allocated.xls";
            SpreadsheetHandler testHandler = new SpreadsheetHandler(filePath);

            Assert.AreEqual("Resources\\Phone Numbers Allocated.xls", testHandler.FilePath);
            Assert.AreEqual(3, testHandler.Workbook.NumberOfSheets);

            Assert.AreEqual(24, testHandler.Sheet.LastRowNum);
            Assert.AreEqual("Sheet1", testHandler.Sheet.SheetName);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestConstructior_ShouldCatchFileNotFoundException()
        {
            string filePath = @"Resources\Invalid File Name.xls";
            new SpreadsheetHandler(filePath);
        }
    }
}
