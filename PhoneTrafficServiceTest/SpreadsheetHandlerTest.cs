using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PhoneTrafficService;
using System.Collections.Generic;
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

        [TestMethod]
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

            SpreadsheetHandler spreadsheetHandler = new SpreadsheetHandler();
            spreadsheetHandler.Workbook = workbook;
            spreadsheetHandler.Sheet = workbook.GetSheetAt(0);

            // Act.
            spreadsheetHandler.SetHeader();

            // Assert.
            ICell trafficHeaderCell = spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(0).GetCell(4);
            Assert.AreEqual("Traffic", trafficHeaderCell.StringCellValue);
        }

        [TestMethod]
        public void TestSetHeader_ShouldCatchException()
        {
            // Arrange.
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0);
            row.CreateCell(1);
            row.CreateCell(2);

            SpreadsheetHandler spreadsheetHandler = new SpreadsheetHandler();
            spreadsheetHandler.Workbook = workbook;
            spreadsheetHandler.Sheet = workbook.GetSheetAt(0);

            // Act.
            spreadsheetHandler.SetHeader();

            // Assert.
            Assert.AreEqual(0, spreadsheetHandler.Workbook.GetSheetAt(0).FirstRowNum);
            Assert.AreEqual(0, spreadsheetHandler.Workbook.GetSheetAt(0).LastRowNum);
            Assert.AreEqual(3, spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(0).LastCellNum);
        }

        [TestMethod]
        public void TestPopulateIncomingCalls_ShouldPopulateIncomingCalls()
        {
            // Arrange.
            HSSFWorkbook testWorkbook = this.GetTestWorkbook();
            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>
            {
                { "1423984271", "158" },
                { "8009843264", "58421" },
                { "1234567890", "0" },
                { "1317462135", "7" }
            };

            SpreadsheetHandler spreadsheetHandler = new SpreadsheetHandler();
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

        private HSSFWorkbook GetTestWorkbook()
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet();

            string[] testSpreadSheetColumnAValues =
            {
                "DDI Number",
                "01423984271",
                null,
                "08009843264",
                "01317462135",
                "984139616874"
            };

            for (int i = 0; i < 6; i++)
            {
                IRow row = sheet.CreateRow(i);
                row.CreateCell(0);
                row.CreateCell(1).SetCellValue(testSpreadSheetColumnAValues[i]);
                row.CreateCell(2);
                row.CreateCell(3);
            }

            return workbook;
        }
    }
}
