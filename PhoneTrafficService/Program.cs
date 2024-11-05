using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;

namespace PhoneTrafficService
{

    /*
     * TODO:
     * 
     * Testing
     * Logging
     * Different format of INCOMING.csv
     * Installer package - how do we get this thing onto the server?
     * MSI Installer?
     */
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static string IncomingFileLocation { get; set; } = ConfigurationManager.AppSettings.Get("IncomingFilePath");
        public static string PhoneNumbersAllocated { get; set; } = ConfigurationManager.AppSettings.Get("PhoneNumbersAllocatedSpreadsheet");

        static Program()
        {
            XmlConfigurator.Configure();
        }

        public static void Main(string[] args)
        {
            log.Info("Start of Application.");
            log.Info($"File location read from config: {IncomingFileLocation}.");

            string[] lines = ReadLinesFromFile(IncomingFileLocation);

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>();

            PopulateIncomingCalls(incomingCallsDictionary, lines);

            SpreadsheetHandler spreadsheetHandler = new SpreadsheetHandler(PhoneNumbersAllocated);
            spreadsheetHandler.SetHeader();
            spreadsheetHandler.PopulateIncomingCalls(incomingCallsDictionary);
            spreadsheetHandler.SaveWorkbook();
        }

        public static string[] ReadLinesFromFile(string path)
        {
            try
            {
                return File.ReadAllLines(path).Skip(1).ToArray();
            }
            catch (Exception exception)
            {
                log.Fatal(exception);
                throw exception;
            }
        }

        public static void PopulateIncomingCalls(Dictionary<string, string> incomingCalls, string[] lines)
        {
            foreach (string line in lines)
            {
                try
                {
                    ProcessCsvLine(incomingCalls, line);
                }
                catch (Exception exception)
                {
                    string errorMessage = $"Error occurred attempting to parse CSV line: {line}.";
                    log.Error(errorMessage);
                    log.Error(exception);
                }
            }
        }

        public static void ProcessCsvLine(Dictionary<string, string> incomingCalls, string line)
        {
            // N.B. It seems there are two formats of INCOMING.CSV now - with one having total number of calls in one column,
            // and the old one had incoming and outgoing calls in separate columns.
            string[] columns = line.Split(',');
            string ddiNumber = columns[0];
            string numberOfCalls = columns[1];
            int numberOfCallsInt;

            if (int.TryParse(numberOfCalls, out numberOfCallsInt))
            {
                incomingCalls.Add(ddiNumber, numberOfCallsInt.ToString());
            }
            else
            {
                log.Error($"Error occurred reading number of calls: {numberOfCalls} is not an integer!");
                incomingCalls.Add(ddiNumber, "0");
            }
        }
    }
}
