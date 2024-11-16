using System.Collections.Generic;
using System.Configuration;
using log4net;
using log4net.Config;
using PhoneTrafficService.CsvFileProcessors;

namespace PhoneTrafficService
{
   /// <summary>
   /// Class containing <c>Main</c> method, which is the main entry point of the application.
   /// </summary>
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static string PhoneNumbersAllocated { get; set; } = ConfigurationManager.AppSettings.Get("PhoneNumbersAllocatedSpreadsheet");
        public static string IncomingFileLocation { get; set; } = ConfigurationManager.AppSettings.Get("IncomingFilePath");
        public static string ApplicationRunMode { get; set; } = ConfigurationManager.AppSettings.Get("RunMode");
        public static int LastNCharacters { get; set; } = -1;
        public static ICsvFileProcessor CsvFileProcessor { get; set; }

        static Program()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// Determines the run mode of the application to decide which CSV file processor logic to use. <br />
        /// Also decides how many 'last characters' to use when attempting to match DDI numbers from the<br />
        /// CSV file to those in Phone Numbers Allocated
        /// </summary>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public static void DetermineRunMode()
        {
            log.Debug($"Determining run mode for application. Value read from configuration: {ApplicationRunMode}.");

            if (ApplicationRunMode == "ALBANY_HOUSE")
            {
                CsvFileProcessor = new AlbanyHouseCsvFileProcessor(IncomingFileLocation);
                LastNCharacters = 7;
            }
            else if (ApplicationRunMode == "HUDSON_HOUSE")
            {
                CsvFileProcessor = new HudsonHouseCsvFileProcessor(IncomingFileLocation);
                LastNCharacters = 10;
            }
            else
            {
                string errorMessage = $"Unable to determine run mode for configuration value: {ApplicationRunMode}. Allowed values are ALBANY_HOUSE and HUDSON_HOUSE";
                log.Fatal(errorMessage);
                throw new ConfigurationErrorsException(errorMessage);
            }

            log.Info($"Run mode successfully determined as {ApplicationRunMode}. CSV File processor successfully initialised as: {CsvFileProcessor.GetType().FullName}.");
        }

        /// <summary>
        /// Main entry point of application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            log.Info("Start of Application.");
            log.Info($"CSV File Location read from configuration: {IncomingFileLocation}.");

            DetermineRunMode();

            string[] lines = CsvFileProcessor.ReadLinesFromFile();

            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>();

            CsvFileProcessor.PopulateIncomingCalls(incomingCallsDictionary, lines);

            SpreadsheetHandler spreadsheetHandler = new SpreadsheetHandler(PhoneNumbersAllocated);
            spreadsheetHandler.SetHeader();
            spreadsheetHandler.PopulateIncomingCalls(incomingCallsDictionary, LastNCharacters);
            spreadsheetHandler.SaveWorkbook();

            log.Info("End of program.");
        }
    }
}
