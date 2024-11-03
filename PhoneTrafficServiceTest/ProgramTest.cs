using PhoneTrafficService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace PhoneTrafficServiceTest
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void TestGetIncomingFilePathFromConfig_ShouldReturnValue()
        {
            string testConfigValue = Program.IncomingFileLocation;
            Assert.AreEqual("test value 123", testConfigValue);
        }

        [TestMethod]
        public void TestGetPhoneNumbersAllocatedSpreadsheetFileLocation_ShouldReturnValue()
        {
            string testConfigValue = Program.PhoneNumbersAllocated;
            Assert.AreEqual("Test Spreadsheet File Location", testConfigValue);
        }

        [TestMethod]
        public void TestMain()
        {
            Program.IncomingFileLocation = @"Resources\INCOMING.csv";
            Program.PhoneNumbersAllocated = @"Resources\Phone Numbers Allocated.xls";

            Program.Main(new string[0]);

            Assert.IsTrue(File.Exists(@"Resources\Phone Numbers Allocated.xls"));

            // Assert that the contents of the spreadsheet are correct.
            HSSFWorkbook testWorkbook;
            using (FileStream fileStream = new FileStream(@"Resources\Phone Numbers Allocated.xls", FileMode.Open, FileAccess.Read))
            {
                testWorkbook = new HSSFWorkbook(fileStream);
                fileStream.Close();
            }

            ISheet sheet = testWorkbook.GetSheetAt(0);

            string[] expectedContents = this.GetExpectedSpreadsheetFileContents();

            for (int i = 0; i < expectedContents.Length; i++)
            {
                IRow row = sheet.GetRow(i);

                string expectedCellContents = expectedContents[i];
                string spreadsheetValue = row.GetCell(4).StringCellValue;
                Assert.AreEqual(expectedCellContents, spreadsheetValue);
            }
        }

        private string[] GetExpectedSpreadsheetFileContents()
        {
            return new string[]
            {
                "Traffic",
                "13",
                "54",
                "42",
                "89",
                "0",
                "0",
                "0",
                "0",
                "253",
                "126",
                "105",
                "0",
                "45",
                "0",
                "7",
                "300",
                "0",
                "13",
                "0",
                "0",
                "3",
                "4",
                "25"
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            HSSFWorkbook testWorkbook;
            using (FileStream fileStream = new FileStream(@"Resources\Phone Numbers Allocated.xls", FileMode.Open, FileAccess.Read))
            {
                testWorkbook = new HSSFWorkbook(fileStream);
                fileStream.Close();
            }

            ISheet sheet = testWorkbook.GetSheetAt(0);

            for (int i = 0; i < sheet.LastRowNum; i++)
            {
                try
                {
                    sheet.GetRow(i).GetCell(4).SetCellValue(string.Empty);
                }
                catch
                {

                }
            }

            using (FileStream fileStream = new FileStream(@"Resources\Phone Numbers Allocated.xls", FileMode.Open, FileAccess.Write))
            {
                testWorkbook.Write(fileStream);
                fileStream.Close();
            }
        }
    }
}
