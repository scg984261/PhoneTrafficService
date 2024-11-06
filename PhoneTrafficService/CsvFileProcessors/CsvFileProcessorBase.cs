using System;
using System.Configuration;
using System.IO;
using System.Linq;
using log4net;

namespace PhoneTrafficService.CsvFileProcessors
{
    public class CsvFileProcessorBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CsvFileProcessorBase));
        public string IncomingFileLocation { get; set; } = ConfigurationManager.AppSettings.Get("IncomingFilePath");

        public CsvFileProcessorBase()
        {
            log.Info($"Incoming CSV File location read from configuration: {this.IncomingFileLocation}.");
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
