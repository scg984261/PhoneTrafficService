using System.Collections.Generic;
using System.Configuration;
using log4net;
using log4net.Config;
using PhoneTrafficService.CsvFileProcessors;

namespace PhoneTrafficService
{

    /*
     * TODO:
     * 
     * Different format of INCOMING.csv
     * Installer package - how do we get this thing onto the server?
     * MSI Installer?
     */
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static string PhoneNumbersAllocated { get; set; } = ConfigurationManager.AppSettings.Get("PhoneNumbersAllocatedSpreadsheet");
        public static string ApplicationRunMode { get; set; } = ConfigurationManager.AppSettings.Get("RunMode");
        public static ICsvFileProcessor CsvFileProcessor { get; set; }

        static Program()
        {
            XmlConfigurator.Configure();
        }

        public static void DetermineRunMode()
        {
            log.Debug($"Determining run mode for application. Value read from configuration: {ApplicationRunMode}.");

            RunMode runMode;

            if (ApplicationRunMode == "ALBANY_HOUSE")
            {
                runMode = RunMode.ALBANY_HOUSE;
                CsvFileProcessor = new AlbanyHouseCsvFileProcessor();
            }
            else if (ApplicationRunMode == "HUDSON_HOUSE")
            {
                runMode = RunMode.HUDSON_HOUSE;
                CsvFileProcessor = new HudsonHouseCsvFileProcessor();
            }
            else
            {
                string errorMessage = $"Unable to determine run mode for configuration value: {ApplicationRunMode}. Allowed values are ALBANY_HOUSE and HUDSON_HOUSE";
                log.Fatal(errorMessage);
                throw new ConfigurationErrorsException(errorMessage);
            }

            log.Info($"Run mode successfully determined as {runMode}.");
        }

        public static void Main(string[] args)
        {
            log.Info("Start of Application.");

            DetermineRunMode();

            string[] lines = CsvFileProcessor.ReadLinesFromFile();

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>();

            CsvFileProcessor.PopulateIncomingCalls(incomingCallsDictionary, lines);

            SpreadsheetHandler spreadsheetHandler = new SpreadsheetHandler(PhoneNumbersAllocated);
            spreadsheetHandler.SetHeader();
            spreadsheetHandler.PopulateIncomingCalls(incomingCallsDictionary);
            spreadsheetHandler.SaveWorkbook();

            log.Info("End of program.");
        }
    }
}
