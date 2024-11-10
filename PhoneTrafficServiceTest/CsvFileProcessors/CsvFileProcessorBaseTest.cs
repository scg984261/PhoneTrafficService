using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhoneTrafficService.CsvFileProcessors;
using System.IO;

namespace PhoneTrafficServiceTest.CsvFileProcessors
{
    [TestClass]
    public class CsvFileProcessorBaseTest
    {
        [TestMethod]
        public void TestConstructor_ShouldPopulateIncomingFileLocation()
        {
            CsvFileProcessorBase testCsvFileProcessor = new CsvFileProcessorBase(@"Resources\INCOMING.CSV");
            Assert.AreEqual(@"Resources\INCOMING.CSV", testCsvFileProcessor.IncomingFileLocation);
        }

        [TestMethod]
        public void TestReadLinesFromFile_ShouldReturnLines()
        {
            string filePath = @"Resources\TestFile.csv";
            CsvFileProcessorBase testCsvFileProcessor = new CsvFileProcessorBase(filePath);

            string[] fileContents = testCsvFileProcessor.ReadLinesFromFile();

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
            CsvFileProcessorBase testCsvFileProcessor = new CsvFileProcessorBase(filePath);

            testCsvFileProcessor.ReadLinesFromFile();
        }
    }
}
