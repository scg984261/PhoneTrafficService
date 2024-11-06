using System.Collections.Generic;
using System;
using log4net;

namespace PhoneTrafficService.CsvFileProcessors
{
    public class AlbanyHouseCsvFileProcessor : CsvFileProcessorBase, ICsvFileProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AlbanyHouseCsvFileProcessor));

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
            // N.B. It seems there are two formats of INCOMING.CSV now - with one having total number of calls in one column,
            // and the old one had incoming and outgoing calls in separate columns.
            string[] columns = line.Split(',');
            string ddiNumber = columns[0];
            string numberOfCalls = columns[1];
            int numberOfCallsInt;

            if (int.TryParse(numberOfCalls, out numberOfCallsInt))
            {
                dictionary.Add(ddiNumber, numberOfCallsInt.ToString());
                log.Debug($"Processed CSV line. DDI number: {ddiNumber}. Number of calls: {numberOfCalls}.");
            }
            else
            {
                log.Error($"Error occurred reading number of calls: {numberOfCalls} is not an integer!");
                dictionary.Add(ddiNumber, "0");
            }
        }
    }
}
