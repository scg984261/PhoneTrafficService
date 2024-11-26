using System.Collections.Generic;
using log4net;

namespace PhoneTrafficService.CsvFileProcessors
{
    /// <summary>
    /// <b><c>Class</c></b> containing <b>Hudson House</b> implementations for processing CSV file containing Phone Traffic data.
    /// </summary>
    public class HudsonHouseCsvFileProcessor : CsvFileProcessorBase, ICsvFileProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HudsonHouseCsvFileProcessor));

        /// <summary>
        /// Default no Args constructor for <c>HudsonHouseCsvFileProcessor</c>. Sets location of the incoming file location to an empty string.
        /// </summary>
        public HudsonHouseCsvFileProcessor() : base(string.Empty)
        {
        }

        /// <summary>
        /// Constructs a <c>HudsonHouseCsvFileProcessor</c>. Sets the location of the CSV File to be processed to the string passed in as argument.        /// </summary>
        /// <param name="incomingFileLocaiton">Location of CSV file to be processed.</param>
        public HudsonHouseCsvFileProcessor(string incomingFileLocaiton) : base(incomingFileLocaiton)
        {
        }

        /// <summary>
        /// Processes the CSV line passed in as argument and determines the DDI number and the number of calls from that line.<br />
        /// <b>The last 10 digits of the DDI number</b> should be given in <b>column A</b>,<br />
        /// and the <b>number of calls</b> should be given as an <c>int</c> in <b>column B</b>.<br />
        /// If the number of calls is a valid integer value, an entry is added to the <c>dictionary</c> with DDI number as the <c>key</c> <br />
        /// and number of calls in <c>string</c> format as the value.<br />
        /// If not a valid int, then <b><c>0</c></b> is used as a placeholder value.
        /// </summary>
        /// <param name="dictionary">Dictionary mapping DDI numbers to a <c>string</c> representation of the number of calls.</param>
        /// <param name="line">Comma separated string representing a line from a CSV file.<br />Should contain the DDI number and the number of calls.</param>
        public override void ProcessCsvLine(Dictionary<string, string> dictionary, string line)
        {
            // In Hudson house format,
            // The DDI number is in column A and only the last 10 digits are given.
            // The total number of calls is given in column B.
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
