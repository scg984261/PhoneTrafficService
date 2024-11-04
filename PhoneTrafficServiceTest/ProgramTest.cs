using System;
using System.Collections.Generic;
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

        [TestMethod]
        public void TestReadLinesFromFile_ShouldReturnLines()
        {
            string filePath = @"Resources\TestFile.csv";

            string[] fileContents = Program.ReadLinesFromFile(filePath);

            Assert.AreEqual(3, fileContents.Length);

            Assert.AreEqual("1138800487,13,04/06/2024 14:24,04/06/2024 14:24", fileContents[0]);
            Assert.AreEqual("1138800843,54,25/04/2024 11:23,23/07/2024 12:19", fileContents[1]);
            Assert.AreEqual("1156719835,42,24/04/2024 08:59,22/07/2024 09:31", fileContents[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestReadLinesFromFile_ShouldThrowException()
        {
            string filePath = @"InvalidFileNameShouldThrowException.csv";
            Program.ReadLinesFromFile(filePath);
        }

        [TestMethod]
        public void TestPopulateIncomingCalls()
        {
            string[] incomingCallsData =
            {
                "1138800487,13,04/06/2024 14:24,04/06/2024 14:24",
                "1138800843,54,25/04/2024 11:23,23/07/2024 12:19",
                "1126915612,testvaluenotanumber,25/04/2024 11:23,23/07/2024 12:19,25/04/2024 11:23,23/07/2024 12:19",
                "1156719835,42,24/04/2024 08:59,22/07/2024 09:31"
            };

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>();

            Program.PopulateIncomingCalls(incomingCallsDictionary, incomingCallsData);

            Assert.AreEqual(4, incomingCallsDictionary.Count);

            string numberOfCalls;

            Assert.IsTrue(incomingCallsDictionary.TryGetValue("1138800487", out numberOfCalls));
            Assert.AreEqual("13", numberOfCalls);

            Assert.IsTrue(incomingCallsDictionary.TryGetValue("1138800843", out numberOfCalls));
            Assert.AreEqual("54", numberOfCalls);

            Assert.IsTrue(incomingCallsDictionary.TryGetValue("1126915612", out numberOfCalls));
            Assert.AreEqual("0", numberOfCalls);

            Assert.IsTrue(incomingCallsDictionary.TryGetValue("1156719835", out numberOfCalls));
            Assert.AreEqual("42", numberOfCalls);
        }

        [TestMethod]
        public void TestPopulateIncomingCalls_ShouldCatchException()
        {
            string[] incomingCallsData =
            {
                "1138800487,13,04/06/2024 14:24,04/06/2024 14:24",
                "1138800843;54;25/04/2024 11:23;23/07/2024 12:19",
            };

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>();

            Program.PopulateIncomingCalls(incomingCallsDictionary, incomingCallsData);

            Assert.AreEqual(1, incomingCallsDictionary.Count);

            string numberOfCalls;

            Assert.IsTrue(incomingCallsDictionary.TryGetValue("1138800487", out numberOfCalls));
            Assert.AreEqual("13", numberOfCalls);

            Assert.IsFalse(incomingCallsDictionary.TryGetValue("1138800843", out numberOfCalls));
            Assert.IsNull(numberOfCalls);
        }

        [TestMethod]
        public void TestParseCsvLine_ShouldAddToDictionary()
        {
            Dictionary<string, string> incomingCalls = new Dictionary<string, string>();

            string line = "1218096874,300,24/04/2024 10:35,23/07/2024 13:08";

            Program.ProcessCsvLine(incomingCalls, line);

            string numberOfCalls;

            Assert.AreEqual(1, incomingCalls.Count);

            Assert.IsTrue(incomingCalls.TryGetValue("1218096874", out numberOfCalls));

            Assert.AreEqual("300", numberOfCalls);
        }

        [TestMethod]
        public void TestParseCsvLine_NumberOfCallsNotInt_ShouldAddZeroAsPlaceholder()
        {
            Dictionary<string, string> incomingCalls = new Dictionary<string, string>();

            string line = "1203489412,notanumber,24/04/2024 10:35,23/07/2024 13:08";

            Program.ProcessCsvLine(incomingCalls, line);

            Assert.AreEqual(1, incomingCalls.Count);

            string numberOfCalls;

            Assert.IsTrue(incomingCalls.TryGetValue("1203489412", out numberOfCalls));

            Assert.AreEqual("0", numberOfCalls);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestParseCsvLine_ShouldThrowOutOfBoundsException()
        {
            string line = "1218096874";
            Program.ProcessCsvLine(new Dictionary<string, string>(), line);
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
