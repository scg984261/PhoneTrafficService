using NUnit.Framework;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PhoneTrafficService.SpreadsheetFileHandlers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhoneTrafficServiceTest.SpreadsheetFileHandlers;

namespace PhoneTrafficServiceTest
{
    public class DefaultSpreadsheetHandlerTest
    {
        private static readonly string testDirectory = TestContext.CurrentContext.TestDirectory;

        [Test]
        public void TestConstructor()
        {
            string filePath = $@"{testDirectory}\Resources\Phone Numbers Allocated.xls";
            DefaultSpreadsheetHandler testHandler = new DefaultSpreadsheetHandler(filePath);

            string[] stringArray = testHandler.FilePath.Split('\\');
            string fileName = stringArray.Last();
            string folderName = stringArray.Reverse().Skip(1).First();

            Assert.AreEqual("Phone Numbers Allocated.xls", fileName);
            Assert.AreEqual("Resources", folderName);

            Assert.AreEqual(3, testHandler.Workbook.NumberOfSheets);

            Assert.AreEqual(24, testHandler.Sheet.LastRowNum);
            Assert.AreEqual("Sheet1", testHandler.Sheet.SheetName);
        }

        [Test]
        public void TestConstructior_ShouldCatchFileNotFoundException()
        {
            string filePath = $@"{testDirectory}\Resources\Invalid File Name.xls";
            Assert.That(() => new DefaultSpreadsheetHandler(filePath), Throws.Exception.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void TestSetHeader_ShouldSetHeader()
        {
            // Arrange.
            HSSFWorkbook workbook = new HSSFWorkbook();
            workbook.CreateSheet();

            ISheet sheet = workbook.GetSheetAt(0);

            IRow row = sheet.CreateRow(0);
            row.CreateCell(0);
            row.CreateCell(1);
            row.CreateCell(2);
            row.CreateCell(3);
            row.CreateCell(4);

            DefaultSpreadsheetHandler spreadsheetHandler = new DefaultSpreadsheetHandler();
            spreadsheetHandler.Workbook = workbook;
            spreadsheetHandler.Sheet = workbook.GetSheetAt(0);

            // Act.
            spreadsheetHandler.SetHeader();

            // Assert.
            ICell trafficHeaderCell = spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(0).GetCell(4);
            Assert.AreEqual("Traffic", trafficHeaderCell.StringCellValue);
        }

        [Test]
        public void TestSetHeader_ShouldCatchException()
        {
            // Arrange.
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0);
            row.CreateCell(1);
            row.CreateCell(2);

            DefaultSpreadsheetHandler spreadsheetHandler = new DefaultSpreadsheetHandler();
            spreadsheetHandler.Workbook = workbook;
            spreadsheetHandler.Sheet = workbook.GetSheetAt(0);

            // Act.
            spreadsheetHandler.SetHeader();

            // Assert.
            Assert.AreEqual(0, spreadsheetHandler.Workbook.GetSheetAt(0).FirstRowNum);
            Assert.AreEqual(0, spreadsheetHandler.Workbook.GetSheetAt(0).LastRowNum);
            Assert.AreEqual(3, spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(0).LastCellNum);
        }

        [Test]
        public void TestPopulateIncomingCalls_ShouldPopulateIncomingCalls_HudsonHouseFormat()
        {
            // Arrange.
            string[] testSpreadSheetValues =
             {
                "DDI Number",
                "01423984271",
                null,
                "08009843264",
                "01317462135",
                "984139616874"
            };

            HSSFWorkbook testWorkbook = SpreadsheetUtils.CreateTestWorkbook(testSpreadSheetValues);
            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>
            {
                { "01423984271", "158" },
                { "08009843264", "58421" },
                { "01234567890", "0" },
                { "01317462135", "7" }
            };

            DefaultSpreadsheetHandler spreadsheetHandler = new DefaultSpreadsheetHandler();
            spreadsheetHandler.Workbook = testWorkbook;
            spreadsheetHandler.Sheet = testWorkbook.GetSheetAt(0);

            // Act.
            spreadsheetHandler.PopulateIncomingCalls(incomingCallsDictionary);

            // Assert.
            Assert.AreEqual("158", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(1).GetCell(4).StringCellValue);
            Assert.IsNull(spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(2).GetCell(4));
            Assert.AreEqual("58421", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(3).GetCell(4).StringCellValue);
            Assert.AreEqual("7", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(4).GetCell(4).StringCellValue);
            Assert.AreEqual("0", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(5).GetCell(4).StringCellValue);
        }

        [Test]
        public void TestGetDdiNumberFromRow_ShouldReturnDdiNumber()
        {
            // Arrange.
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("Test Value.");
            row.CreateCell(1).SetCellValue("013169730324");

            DefaultSpreadsheetHandler spreadsheetHandler = new DefaultSpreadsheetHandler();

            // Act.
            string ddiNumber = spreadsheetHandler.GetDdiNumberFromRow(row);

            // Assert.
            Assert.AreEqual("013169730324", ddiNumber);
        }

        [Test]
        public void TestGetDdiNumberFromRow_ShouldCatchExceptionAndReturnEmptyString()
        {
            // Arrange.
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("Test Value.");

            DefaultSpreadsheetHandler spreadsheetHandler = new DefaultSpreadsheetHandler();

            // Act.
            string ddiNumber = spreadsheetHandler.GetDdiNumberFromRow(row);

            // Assert.
            Assert.AreEqual(string.Empty, ddiNumber);
        }

        [Test]
        public void TestPopulateTraffic_ShouldPopulateCellWithIncomingCalls()
        {
            // Arrange.
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow row = sheet.CreateRow(0);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(string.Empty);

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>
            {
                { "123446", "984" },
                { "98621347", "38425" },
                { "54832756", "948425" }
            };

            string ddiNumber = "98621347";

            DefaultSpreadsheetHandler spreadsheethandler = new DefaultSpreadsheetHandler();

            // Act.
            spreadsheethandler.PopulateTraffic(incomingCallsDictionary, ddiNumber, cell);

            // Assert.
            Assert.AreEqual("38425", cell.StringCellValue);
        }

        [Test]
        public void TestPopulateTraffic_ShouldPopulateCellWithPlaceHolderValue()
        {
            // Arrange.
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow row = sheet.CreateRow(0);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(string.Empty);

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>
            {
                { "123446", "984" },
                { "98621348", "38425" },
                { "54832756", "948425" }
            };

            string ddiNumber = "98621347";

            DefaultSpreadsheetHandler spreadsheethandler = new DefaultSpreadsheetHandler();

            // Act.
            spreadsheethandler.PopulateTraffic(incomingCallsDictionary, ddiNumber, cell);

            // Assert.
            Assert.AreEqual("0", cell.StringCellValue);
        }
    }
}
