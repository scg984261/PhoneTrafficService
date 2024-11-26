using PhoneTrafficService.CsvFileProcessors;
using NUnit.Framework;
using System.Collections.Generic;

namespace PhoneTrafficServiceTest.CsvFileProcessors
{
    public class AlbanyHouseCsvFileProcessorTest
    {
        [Test]
        public void TestConstructor()
        {
            AlbanyHouseCsvFileProcessor fileProcessor = new AlbanyHouseCsvFileProcessor("Test value.");
            Assert.AreEqual("Test value.", fileProcessor.IncomingFileLocation);
        }

        [Test]
        public void TestConstructor_NoArgs()
        {
            AlbanyHouseCsvFileProcessor fileProcessor = new AlbanyHouseCsvFileProcessor();
            Assert.AreEqual(string.Empty, fileProcessor.IncomingFileLocation);
        }

        [Test]
        public void TestPopulateIncomingCalls_DdiNumberHasInvertedCommas()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            AlbanyHouseCsvFileProcessor testFileProcessor = new AlbanyHouseCsvFileProcessor();

            string[] lines =
            {
                "\"\",\"** Unallocated **\",1,0,0,0,0,0.00,0,85,85",
                "\"1400140\",\"Stonbury Ltd\",9,9,13,0,2,18.18,22,689,77",
                "\"2140684\",\"Ashton Gas\",38,12,30,0,3,7.32,30,2835,75",
                "\"2234661\",\"Andrews Removals\",249,11,31,10,20,7.43,28,25070,97",
                "\"2460023\",\"Inovix Network Solutions Ltd\",42,14,31,0,2,4.55,7,1812,43",
                "\"2460749\",\"Crete Escapes Ltd\",65,10,31,0,2,2.99,26,4732,73",
                "\"2607434\",\"Waterfox Consultancy Ltd\",48,10,31,13,13,21.31,24,7013,115",
                "\"2703700\",\"Rhino\",28,10,25,1,4,12.50,30,1692,58",
                "\"2703701\",\"Cash Tills Direct\",247,11,31,0,6,2.37,30,18822,76",
                "\"2703702\",\"Dean Associates\",53,11,31,0,1,1.85,19,3191,60",
            };

            testFileProcessor.PopulateIncomingCalls(dictionary, lines);

            Assert.AreEqual(10, dictionary.Count);

            string numberOfCalls;

            Assert.IsTrue(dictionary.TryGetValue("", out numberOfCalls));
            Assert.AreEqual("1", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("1400140", out numberOfCalls));
            Assert.AreEqual("11", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2140684", out numberOfCalls));
            Assert.AreEqual("41", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2234661", out numberOfCalls));
            Assert.AreEqual("269", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2460023", out numberOfCalls));
            Assert.AreEqual("44", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2460749", out numberOfCalls));
            Assert.AreEqual("67", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2607434", out numberOfCalls));
            Assert.AreEqual("61", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2703700", out numberOfCalls));
            Assert.AreEqual("32", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2703701", out numberOfCalls));
            Assert.AreEqual("253", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("2703702", out numberOfCalls));
            Assert.AreEqual("54", numberOfCalls);
        }

        [Test]
        public void TestPopulateIncomingCalls_DdiNumberDoesNotHaveInvertedCommas()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            AlbanyHouseCsvFileProcessor testFileProcessor = new AlbanyHouseCsvFileProcessor();

            string[] lines =
            {
                ",** Unallocated **,5,1,5,2,0,0,0,4299,614",
                "578874,,8,8,15,2,1,11.11,27,1007,101",
                "578876,,2,10,13,0,3,60,7,114,57",
                "578877,,5,11,29,1,1,16.67,5,1171,195",
                "578878,,24,9,31,0,1,4,7,1429,60",
                "1061185,Hillview Park Estates,65,11,31,30,1,1.52,13,10783,114",
                "1061186,Hassell Inclusion Ltd,3,8,11,0,2,40,4,31,10",
                "1061187,Dr P Jeczmien,6,15,27,0,2,25,4,401,67",
                "1061188,Dr P Jeczmien,4,19,31,0,2,33.33,7,231,58",
            };

            testFileProcessor.PopulateIncomingCalls(dictionary, lines);

            string numberOfCalls;

            Assert.IsTrue(dictionary.TryGetValue(string.Empty, out numberOfCalls));
            Assert.AreEqual("5", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("578874", out numberOfCalls));
            Assert.AreEqual("9", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("578876", out numberOfCalls));
            Assert.AreEqual("5", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("578877", out numberOfCalls));
            Assert.AreEqual("6", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("578878", out numberOfCalls));
            Assert.AreEqual("25", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("1061185", out numberOfCalls));
            Assert.AreEqual("66", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("1061186", out numberOfCalls));
            Assert.AreEqual("5", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("1061187", out numberOfCalls));
            Assert.AreEqual("8", numberOfCalls);

            Assert.IsTrue(dictionary.TryGetValue("1061188", out numberOfCalls));
            Assert.AreEqual("6", numberOfCalls);
        }

        [Test]
        public void TestProcessCsvLine()
        {
            // Arrange.
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string line = "2715941,Curtainly Elegant Ltd,32,9,22,0,5,13.51,27,1649,52";

            AlbanyHouseCsvFileProcessor testFileProcessor = new AlbanyHouseCsvFileProcessor();

            // Act.
            testFileProcessor.ProcessCsvLine(dictionary, line);

            // Assert.
            Assert.AreEqual(1, dictionary.Count);

            string numberOfCalls;

            Assert.IsTrue(dictionary.TryGetValue("2715941", out numberOfCalls));
            Assert.AreEqual("37", numberOfCalls);
        }

        [Test]
        public void TestCalculateNumberOfCalls_ShouldCalculateNumberOfCalls()
        {
            // Arrange.
            string[] stringArray =
            {
                "3800859",
                "Lucas Grant Lettings",
                "39",
                "9",
                "31",
                "8",
                "1",
                "0",
                "94",
                "2802",
                "72"
            };

            AlbanyHouseCsvFileProcessor testFileProcessor = new AlbanyHouseCsvFileProcessor();

            // Act.
            int numberOfCalls = testFileProcessor.CalculateTotalNumberOfCalls(stringArray);

            // Assert.
            Assert.AreEqual(40, numberOfCalls);
        }

        [Test]
        public void TestCalculateNumberOfCalls_ShouldCatchExceptionAndReturnZero()
        {
            // Arrange.
            string[] stringArray =
            {
                "3800859",
                "Lucas Grant Lettings",
                "39",
                "9",
                "31",
                "8",
                "Invalid value not an int",
                "0",
                "94",
                "2802",
                "72"
            };

            AlbanyHouseCsvFileProcessor testFileProcessor = new AlbanyHouseCsvFileProcessor();

            int numberOfCalls = -1;

            // Act.
            numberOfCalls = testFileProcessor.CalculateTotalNumberOfCalls(stringArray);

            // Assert.
            Assert.AreEqual(0, numberOfCalls);
        }
    }
}
