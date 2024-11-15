using System.IO;
using System.Configuration;
using PhoneTrafficService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PhoneTrafficService.CsvFileProcessors;

namespace PhoneTrafficServiceTest
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void TestGetPhoneNumbersAllocatedSpreadsheetFileLocation_ShouldReturnValue()
        {
            string testConfigValue = Program.PhoneNumbersAllocated;
            Assert.AreEqual("Test Spreadsheet File Location", testConfigValue);
        }

        [TestMethod]
        public void TestDetermineRunMode_ShouldBeAlbanyHouse()
        {
            // Arrange.
            // Simulate reading the application run mode from configuration.
            Program.ApplicationRunMode = "ALBANY_HOUSE";

            // Simulate reading the location of INCOMING.CSV from configuration.
            Program.IncomingFileLocation = "C:\\Test\\Incoming\\File\\Location\\AlbanyHouse";

            // Act.
            Program.DetermineRunMode();

            // Assert.
            Assert.IsTrue(Program.CsvFileProcessor is AlbanyHouseCsvFileProcessor);
            AlbanyHouseCsvFileProcessor fileProcessor = (AlbanyHouseCsvFileProcessor) Program.CsvFileProcessor;
            Assert.AreEqual("C:\\Test\\Incoming\\File\\Location\\AlbanyHouse", fileProcessor.IncomingFileLocation);
            Assert.AreEqual(7, Program.LastNCharacters);
        }

        [TestMethod]
        public void TestDetermineRunMode_ShouldBeHudsonHouse()
        {
            // Arrange.
            // Simulate reading the application run mode from configuration.
            Program.ApplicationRunMode = "HUDSON_HOUSE";

            // Simulate reading the location of INCOMING.CSV from configuration.
            Program.IncomingFileLocation = "C:\\Test\\Incoming\\File\\Location\\HudsonHouse";

            // Act.
            Program.DetermineRunMode();

            // Assert.
            Assert.IsTrue(Program.CsvFileProcessor is HudsonHouseCsvFileProcessor);
            HudsonHouseCsvFileProcessor fileProcessor = (HudsonHouseCsvFileProcessor) Program.CsvFileProcessor;
            Assert.AreEqual("C:\\Test\\Incoming\\File\\Location\\HudsonHouse", fileProcessor.IncomingFileLocation);
            Assert.AreEqual(10, Program.LastNCharacters);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestDetermineRunMode_ShouldThrowException()
        {
            // Arrange.
            Program.ApplicationRunMode = "INVALID_RUN_MODE";
            Program.IncomingFileLocation = "C:\\Test\\Incoming\\File\\Location\\HudsonHouse";

            // Act.
            Program.DetermineRunMode();
        }

        [TestMethod]
        public void TestMain_RunModeHudsonHouse()
        {
            Program.PhoneNumbersAllocated = @"Resources\Phone Numbers Allocated.xls";
            Program.IncomingFileLocation = @"Resources\INCOMING.CSV";
            Program.ApplicationRunMode = "HUDSON_HOUSE";

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

            this.ValidateSheetContents(sheet, expectedContents);
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

        [TestMethod]
        public void TestMain_RunModeAlbanyHouse()
        {
            Program.ApplicationRunMode = "ALBANY_HOUSE";
            Program.IncomingFileLocation = @"Resources\INCOMING_ALBANY_HOUSE_FORMAT.CSV";
            Program.PhoneNumbersAllocated = @"Resources\AH Phone Numbers Allocated.xls";

            Program.Main(new string[0]);

            Assert.IsTrue(File.Exists(@"Resources\AH Phone Numbers Allocated.xls"));

            HSSFWorkbook testWorkbook;
            using (FileStream fileStream = new FileStream(@"Resources\AH Phone Numbers Allocated.xls", FileMode.Open, FileAccess.Read))
            {
                testWorkbook = new HSSFWorkbook(fileStream);
                fileStream.Close();
            }

            ISheet sheet = testWorkbook.GetSheetAt(0);

            string[] expectedSheetContents =
            {
                "Traffic",
                "9",
                "5",
                "6",
                "25",
                "66",
                "5",
                "8",
                "6",
                "16",
                "0",
                "0",
                "0",
                "0",
                "0",
                "0"
            };

            this.ValidateSheetContents(sheet, expectedSheetContents);
        }

        private void ValidateSheetContents(ISheet sheet, string[] expectedSheetContents)
        {
            for (int i = 0; i < expectedSheetContents.Length; i++)
            {
                IRow row = sheet.GetRow(i);

                string expectedCellContents = expectedSheetContents[i];
                string spreadsheetValue = row.GetCell(4).StringCellValue;
                Assert.AreEqual(expectedCellContents, spreadsheetValue);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            string[] excelFiles =
            {
                @"Resources\Phone Numbers Allocated.xls",
                @"Resources\AH Phone Numbers Allocated.xls"
            };

            foreach (string excelFile in excelFiles)
            {
                HSSFWorkbook testWorkbook;
                using (FileStream fileStream = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
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

                using (FileStream fileStream = new FileStream(excelFile, FileMode.Open, FileAccess.Write))
                {
                    testWorkbook.Write(fileStream);
                    fileStream.Close();
                }
            }
        }
    }
}
