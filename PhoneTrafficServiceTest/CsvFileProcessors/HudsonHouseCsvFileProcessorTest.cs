using System;
using System.Collections.Generic;
using NUnit.Framework;
using PhoneTrafficService.CsvFileProcessors;

namespace PhoneTrafficServiceTest.CsvFileProcessors
{
    public class HudsonHouseCsvFileProcessorTest
    {
        [Test]
        public void TestConstructor()
        {
            HudsonHouseCsvFileProcessor fileProcessor = new HudsonHouseCsvFileProcessor("Test value.");
            Assert.AreEqual("Test value.", fileProcessor.IncomingFileLocation);
        }

        [Test]
        public void TestConstructor_NoArgs()
        {
            HudsonHouseCsvFileProcessor fileProcessor = new HudsonHouseCsvFileProcessor();
            Assert.AreEqual(string.Empty, fileProcessor.IncomingFileLocation);
        }

        [Test]
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

            HudsonHouseCsvFileProcessor testCsvFileProcessor = new HudsonHouseCsvFileProcessor();

            testCsvFileProcessor.PopulateIncomingCalls(incomingCallsDictionary, incomingCallsData);

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

        [Test]
        public void TestPopulateIncomingCalls_ShouldCatchException()
        {
            string[] incomingCallsData =
            {
                "1138800487,13,04/06/2024 14:24,04/06/2024 14:24",
                "1138800843;54;25/04/2024 11:23;23/07/2024 12:19",
            };

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>();

            HudsonHouseCsvFileProcessor testCsvFileProcessor = new HudsonHouseCsvFileProcessor();

            testCsvFileProcessor.PopulateIncomingCalls(incomingCallsDictionary, incomingCallsData);

            Assert.AreEqual(1, incomingCallsDictionary.Count);

            string numberOfCalls;

            Assert.IsTrue(incomingCallsDictionary.TryGetValue("1138800487", out numberOfCalls));
            Assert.AreEqual("13", numberOfCalls);

            Assert.IsFalse(incomingCallsDictionary.TryGetValue("1138800843", out numberOfCalls));
            Assert.IsNull(numberOfCalls);
        }

        [Test]
        public void TestProcessCsvLine_ShouldAddToDictionary()
        {
            Dictionary<string, string> incomingCalls = new Dictionary<string, string>();

            string line = "1218096874,300,24/04/2024 10:35,23/07/2024 13:08";

            HudsonHouseCsvFileProcessor testCsvFileProcessor = new HudsonHouseCsvFileProcessor();

            testCsvFileProcessor.ProcessCsvLine(incomingCalls, line);

            string numberOfCalls;

            Assert.AreEqual(1, incomingCalls.Count);

            Assert.IsTrue(incomingCalls.TryGetValue("1218096874", out numberOfCalls));

            Assert.AreEqual("300", numberOfCalls);
        }

        [Test]
        public void TestProcessCsvLine_NumberOfCallsNotInt_ShouldAddZeroAsPlaceholder()
        {
            Dictionary<string, string> incomingCalls = new Dictionary<string, string>();

            string line = "1203489412,notanumber,24/04/2024 10:35,23/07/2024 13:08";

            HudsonHouseCsvFileProcessor testCsvFileProcessor = new HudsonHouseCsvFileProcessor();

            testCsvFileProcessor.ProcessCsvLine(incomingCalls, line);

            Assert.AreEqual(1, incomingCalls.Count);

            string numberOfCalls;

            Assert.IsTrue(incomingCalls.TryGetValue("1203489412", out numberOfCalls));

            Assert.AreEqual("0", numberOfCalls);
        }

        [Test]
        public void TestProcessCsvLine_ShouldThrowOutOfBoundsException()
        {
            string line = "1218096874";
            HudsonHouseCsvFileProcessor testCsvFileProcessor = new HudsonHouseCsvFileProcessor();
            Assert.That(() => testCsvFileProcessor.ProcessCsvLine(new Dictionary<string, string>(), line), Throws.Exception.TypeOf<IndexOutOfRangeException>());
        }
    }
}
