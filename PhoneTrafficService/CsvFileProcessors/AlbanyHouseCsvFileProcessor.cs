using System;
using System.Collections.Generic;
using log4net;

namespace PhoneTrafficService.CsvFileProcessors
{
    /// <summary>
    /// <b><c>Class</c></b> containing Albany House implementations for processing CSV file containing Phone Traffic Data.
    /// </summary>
    public class AlbanyHouseCsvFileProcessor : CsvFileProcessorBase, ICsvFileProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AlbanyHouseCsvFileProcessor));

        /// <summary>
        /// Default no Args constructor for <c>AlbanyHouseCsvFileProcessor</c>. Sets the location of the incoming file to an empty string.
        /// </summary>
        public AlbanyHouseCsvFileProcessor() : base(string.Empty)
        {
        }

        /// <summary>
        /// Constructs an <c>AlbanyHouseCsvFileProcessor</c>, sets the location of the CSV file to be processed to the string passed in as argument.
        /// </summary>
        /// <param name="incomingFilelocation"></param>
        public AlbanyHouseCsvFileProcessor(string incomingFilelocation) : base(incomingFilelocation)
        {
        }

        /// <summary>
        /// Processes the CSV line passed in as argument and determines the DDI number and the number of calls from that line.<br />
        /// <b>The last 7 digits of the DDI number</b> should be given in <b>column A</b>,<br />
        /// Total number of calls is then calulated. An entry is added to the dictionary, with the DDI number as the key and the calculated number of calls is the value.
        /// </summary>
        /// <param name="dictionary">Dictionary mapping DDI numbers to a <c>string</c> representation of the number of calls.</param>
        /// <param name="line">Comma separated string representing a line from a CSV file.<br />Should contain the DDI number and the number of calls.</param>
        public override void ProcessCsvLine(Dictionary<string, string> dictionary, string line)
        {
            string[] stringArray = line.Split(',');
            string ddiNumber = stringArray[0].Replace("\"", string.Empty);
            int numberOfCalls = this.CalculateTotalNumberOfCalls(stringArray);
            log.Debug($"Processed CSV line. DDI number: {ddiNumber}. Number of calls: {numberOfCalls}.");
            dictionary.Add(ddiNumber, numberOfCalls.ToString());
        }

        /// <summary>
        /// Takes in an <c>array</c> of <c>strings</c> and calculates the total number of calls by adding together the incoming and outgoing calls. <br />
        /// <b>Incoming calls</b> is taken from <b>Column C.</b> <br />
        /// <b>Outgoing calls</b> is taken from <b>Column G.</b><br />
        /// If a number of calls cannot be parsed from the element in the array, <b><c>0</c></b> is returned as a placeholder value.
        /// </summary>
        /// <param name="stringArray">An array of <b><c>strings</c></b> derived from comma separated values, containing the incoming and outgoing calls.</param>
        /// <returns>An <b><c>int</c></b> representing the total number of calls.</returns>
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
