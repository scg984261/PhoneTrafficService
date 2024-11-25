using PhoneTrafficService.SpreadsheetFileHandlers;
using NUnit.Framework;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;

namespace PhoneTrafficServiceTest.SpreadsheetFileHandlers
{
    public class AlbanyHouseSpreadsheetHandlerTest
    {
        [Test]
        public void TestPopulateincomingCalls_ShouldPopulateIncomingCalls()
        {
            // Arrange.
            string[] testSpreadSheetValues =
             {
                "DDI Number",
                "38476698501",
                "98445211235",
                null,
                "4969415654",
                "0062313497"
            };

            HSSFWorkbook testWorkbook = SpreadsheetUtils.CreateTestWorkbook(testSpreadSheetValues);
            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>
            {
                { "5211235", "584" },
                { "6698501", "3642975" },
                { "9415654", "55" },
                { "2313497", "0" }
            };

            AlbanyHouseSpreadsheetHandler spreadsheetHandler = new AlbanyHouseSpreadsheetHandler();
            spreadsheetHandler.Workbook = testWorkbook;
            spreadsheetHandler.Sheet = testWorkbook.GetSheetAt(0);

            // Act.
            spreadsheetHandler.PopulateIncomingCalls(incomingCallsDictionary);

            // Assert.
            Assert.AreEqual("3642975", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(1).GetCell(4).StringCellValue);
            Assert.AreEqual("584", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(2).GetCell(4).StringCellValue);
            Assert.IsNull(spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(3).GetCell(4));
            Assert.AreEqual("55", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(4).GetCell(4).StringCellValue);
            Assert.AreEqual("0", spreadsheetHandler.Workbook.GetSheetAt(0).GetRow(5).GetCell(4).StringCellValue);
        }

        [Test]
        [TestCase("ABCDEFGHIJ0123456789", "3456789")]
        [TestCase("58432165974139874651268458", "1268458")]
        [TestCase("ABCDFGG!\"£$^&1236845", "1236845")]
        [TestCase("123", "123")]
        [TestCase("ABCDEF", "ABCDEF")]
        [TestCase("08425987316", "5987316")]
        [TestCase("12389498745", "9498745")]
        [TestCase("59742568946", "2568946")]
        public void TestGetLast7Characters_ShouldReturnLastNCharacters(string testString, string expected)
        {
            // Arrange.
            AlbanyHouseSpreadsheetHandler spreadsheetHandler = new AlbanyHouseSpreadsheetHandler();

            // Act.
            string lastDigits = spreadsheetHandler.GetLast7Characters(testString);

            // Assert.
            Assert.AreEqual(expected, lastDigits);
        }
    }
}
