using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using log4net;

namespace PhoneTrafficService.CsvFileProcessors
{
    public class CsvFileProcessorBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CsvFileProcessorBase));
        public string IncomingFileLocation { get; set; }

        /// <summary>
        /// Creates a new <b><c>CsvFileProcessorBase</c></b>, with the <c>string</c> passed in as the location of the CSV file to be processed.
        /// </summary>
        /// <param name="incomingFileLocation">String representation of the location on disc containing the CSV file to be processed.</param>
        public CsvFileProcessorBase(string incomingFileLocation)
        {
            log.Info($"Initialising CSV File processor with incoming file location: {incomingFileLocation}.");
            this.IncomingFileLocation = incomingFileLocation;
        }

        /// <summary>
        /// Reads all lines <b>Except the first line</b> from the file specified as <b><c>IncomingFileLocation</c></b>.
        /// </summary>
        /// <returns>
        /// An <c>Array</c> of <b><c>strings</c></b>, where each string represents a line from from the CSV file.
        /// </returns>
        /// <exception cref="Exception">
        /// Logged as Fatal then thrown if an exception is caught attempting to read lines, for example if the file does not exist locally on disc.
        /// </exception>
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

        /// <summary>
        /// Populates the <c>dictionary</c> based on the <c>array</c> of <c>strings</c> passed in.<br/>
        /// Loops through the array of strings an adds an entry to the dictionary for each component.
        /// </summary>
        /// <param name="dictionary">Dictionary mapping the DDI number to the number of calls.</param>
        /// <param name="lines">An <b><c>array</c></b> containing the lines to be processed.</param>
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

        public virtual void ProcessCsvLine(Dictionary<string, string> dictionary, string line)
        {
            NotImplementedException notImplementedException = new NotImplementedException("PROCESS CSV LINE SHOULD NOT BE CALLED ON THE BASE CLASS!");
            log.Error(notImplementedException);
        }
    }
}
