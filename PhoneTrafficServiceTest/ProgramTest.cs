using NUnit.Framework;
using System.Configuration;
using System.IO;
using PhoneTrafficService;
using PhoneTrafficService.CsvFileProcessors;
using PhoneTrafficService.SpreadsheetFileHandlers;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;


namespace PhoneTrafficServiceTest
{
    public class ProgramTest
    {
        private static readonly string testDirectory = TestContext.CurrentContext.TestDirectory;

        [Test]
        public void TestGetPhoneNumbersAllocatedSpreadsheetFileLocation_ShouldReturnValue()
        {
            string testConfigValue = Program.PhoneNumbersAllocated;
            Assert.AreEqual("Test Spreadsheet File Location", testConfigValue);
        }

        [Test]
        public void TestDetermineRunMode_ShouldBeAlbanyHouse()
        {
            // Arrange.
            // Simulate reading the application run mode from configuration.
            Program.ApplicationRunMode = "ALBANY_HOUSE";

            // Simulate reading the location of INCOMING.CSV from configuration.
            Program.IncomingFileLocation = "C:\\Test\\Incoming\\File\\Location\\AlbanyHouse";

            Program.PhoneNumbersAllocated = $@"{testDirectory}\Resources\AH Phone Numbers Allocated.xls";

            // Act.
            Program.DetermineRunMode();

            // Assert.
            Assert.IsTrue(Program.CsvFileProcessor is AlbanyHouseCsvFileProcessor);
            AlbanyHouseCsvFileProcessor fileProcessor = (AlbanyHouseCsvFileProcessor)Program.CsvFileProcessor;
            Assert.AreEqual("C:\\Test\\Incoming\\File\\Location\\AlbanyHouse", fileProcessor.IncomingFileLocation);

            Assert.IsTrue(Program.SpreadsheetHandler is AlbanyHouseSpreadsheetHandler);
            Assert.IsTrue(Program.SpreadsheetHandler.FilePath.EndsWith("\\Resources\\AH Phone Numbers Allocated.xls"));
        }

        [Test]
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
            HudsonHouseCsvFileProcessor fileProcessor = (HudsonHouseCsvFileProcessor)Program.CsvFileProcessor;
            Assert.AreEqual("C:\\Test\\Incoming\\File\\Location\\HudsonHouse", fileProcessor.IncomingFileLocation);
        }

        [Test]
        public void TestDetermineRunMode_ShouldThrowException()
        {
            // Arrange.
            Program.ApplicationRunMode = "INVALID_RUN_MODE";
            Program.IncomingFileLocation = "C:\\Test\\Incoming\\File\\Location\\HudsonHouse";

            // Act.
            Assert.That(() => Program.DetermineRunMode(), Throws.Exception.TypeOf<ConfigurationErrorsException>());
        }

        [Test]
        public void TestMain_RunModeHudsonHouse()
        {
            Program.PhoneNumbersAllocated = $@"{testDirectory}\Resources\Phone Numbers Allocated.xls";
            Program.IncomingFileLocation = $@"{testDirectory}\Resources\INCOMING.CSV";
            Program.ApplicationRunMode = "HUDSON_HOUSE";

            Program.Main(new string[0]);

            Assert.IsTrue(File.Exists($@"{testDirectory}\Resources\Phone Numbers Allocated.xls"));

            // Assert that the contents of the spreadsheet are correct.
            HSSFWorkbook testWorkbook;
            using (FileStream fileStream = new FileStream($@"{testDirectory}\Resources\Phone Numbers Allocated.xls", FileMode.Open, FileAccess.Read))
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

        [Test]
        public void TestMain_RunModeAlbanyHouse()
        {
            Program.ApplicationRunMode = "ALBANY_HOUSE";
            Program.IncomingFileLocation = $@"{testDirectory}\Resources\INCOMING_ALBANY_HOUSE_FORMAT.CSV";
            Program.PhoneNumbersAllocated = $@"{testDirectory}\Resources\AH Phone Numbers Allocated.xls";

            Program.Main(new string[0]);

            Assert.IsTrue(File.Exists($@"{testDirectory}\Resources\AH Phone Numbers Allocated.xls"));

            HSSFWorkbook testWorkbook;
            using (FileStream fileStream = new FileStream($@"{testDirectory}\Resources\AH Phone Numbers Allocated.xls", FileMode.Open, FileAccess.Read))
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

        [TearDown]
        public void Cleanup()
        {
            string[] excelFiles =
            {
                $@"{testDirectory}\Resources\Phone Numbers Allocated.xls",
                $@"{testDirectory}\Resources\AH Phone Numbers Allocated.xls"
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
