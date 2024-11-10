using System;
using System.Collections.Generic;
using log4net;

namespace PhoneTrafficService.CsvFileProcessors
{
    public class AlbanyHouseCsvFileProcessor : CsvFileProcessorBase, ICsvFileProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AlbanyHouseCsvFileProcessor));

        public AlbanyHouseCsvFileProcessor() : base(string.Empty)
        {

        }

        public AlbanyHouseCsvFileProcessor(string incomingFilelocation) : base(incomingFilelocation)
        {
        }

        public void PopulateIncomingCalls(Dictionary<string, string> dictionary, string[] lines)
        {
            log.Debug($"Processing CSV lines. {lines.Length} to process.");

            foreach (string line in lines)
            {
                try
                {
                    log.Debug($"Processing CSV line: {line}.");
                    this.ProcessCsvLine(dictionary, line);
                }
                catch (Exception exception)
                {
                    string errorMessage = $"Error occurred attempting to parse CSV line: {line}.";
                    log.Error(errorMessage);
                    log.Error(exception);
                }
            }

            log.Info("Finished processing CSV file.");
            log.Debug($"Finished processing CSV file. {dictionary.Count} lines processed.");
        }

        public void ProcessCsvLine(Dictionary<string, string> dictionary, string line)
        {
            string[] stringArray = line.Split(',');
            string ddiNumber = stringArray[0].Replace("\"", string.Empty);
            int numberOfCalls = this.CalculateTotalNumberOfCalls(stringArray);
            log.Debug($"Processed CSV line. DDI number: {ddiNumber}. Number of calls: {numberOfCalls}.");
            dictionary.Add(ddiNumber, numberOfCalls.ToString());
        }

        public int CalculateTotalNumberOfCalls(string[] stringArray)
        {
            try
            {
                int incomingCalls = int.Parse(stringArray[2]);
                int outgoingCalls = int.Parse(stringArray[6]);
                return incomingCalls + outgoingCalls;
            }
            catch (Exception exception)
            {
                log.Error($"Error occurred attempting process line from CSV file: {stringArray}.");
                log.Error(exception);
                return 0;
            }
        }
    }
}
