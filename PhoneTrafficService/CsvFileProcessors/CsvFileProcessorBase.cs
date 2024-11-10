using System;
using System.IO;
using System.Linq;
using log4net;

namespace PhoneTrafficService.CsvFileProcessors
{
    public class CsvFileProcessorBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CsvFileProcessorBase));
        public string IncomingFileLocation { get; set; }

        public CsvFileProcessorBase(string incomingFileLocation)
        {
            log.Info($"Initialising CSV File processor with incoming file location: {incomingFileLocation}.");
            this.IncomingFileLocation = incomingFileLocation;
        }

        public string[] ReadLinesFromFile()
        {
            try
            {
                return File.ReadAllLines(this.IncomingFileLocation).Skip(1).ToArray();
            }
            catch (Exception exception)
            {
                log.Fatal(exception);
                throw exception;
            }
        }
    }
}
