using System;
using System.IO;
using System.Collections.Generic;
using log4net;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace PhoneTrafficService
{
    /// <summary>
    /// <c>Class</c> containing methods for handling logic to manipulate spreadsheets.
    /// </summary>
    public class SpreadsheetHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SpreadsheetHandler));

        public string FilePath { get; set; }
        public HSSFWorkbook Workbook { get; set; }
        public ISheet Sheet { get; set; }

        /// <summary>
        /// Default no-args constructor.
        /// </summary>
        public SpreadsheetHandler()
        {
        }

        /// <summary>
        /// Constructs a <b><c>SpreadsheetHandler</c></b>. Uses the path provided to initialise an <b><c>HSSFWorkbook</c></b>.
        /// </summary>
        /// <param name="spreadsheetFilePath">Path of the spreadsheet that this <b><c>SpreadsheetHandler</c></b> will work with.</param>
        /// <exception cref="Exception">Thrown if an <b><c>Exception</c></b> is caught attempting to initialise an <b><c>HSSFWorkbook</c></b> using the path provided.</exception>
        public SpreadsheetHandler(string spreadsheetFilePath)
        {
            log.Debug($"Attempting to create Spreadsheet Handler with path: {spreadsheetFilePath}.");
            this.FilePath = spreadsheetFilePath;

            try
            {
                using (FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read))
                {
                    this.Workbook = new HSSFWorkbook(fileStream);
                    fileStream.Close();
                }

                this.Sheet = this.Workbook.GetSheetAt(0);
            }
            catch (Exception exception)
            {
                log.Error("Error occurred attempting to create Spreadsheet handler!");
                log.Fatal(exception);
                throw exception;
            }
            finally
            {
                log.Debug("Spreadsheet handler successfully created.");
            }
        }

        /// <summary>
        /// Sets the header of the spreadsheet in the Traffic column to "Traffic".
        /// </summary>
        public void SetHeader()
        {
            try
            {
                IRow headerRow = this.Sheet.GetRow(0);
                ICell trafficHeaderCell = headerRow.GetCell(4);

                if (trafficHeaderCell.StringCellValue != "Traffic")
                {
                    trafficHeaderCell.SetCellValue("Traffic");
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"Error occurred attempting to set the value of the header row in spreadsheet: {this.FilePath}";
                log.Error(errorMessage);
                log.Error(exception);
            }
        }

        /// <summary>
        /// Populates the incoming calls Column of the Spreadsheet using data from the incoming calls dictionary. <br />
        /// </summary>
        /// <param name="incomingCallsDictionary"><b><c>Dictionary</c></b> containing the DDI numbers and number of call for each one.</param>
        /// <param name="numberOfDigits">The number of digits that will be used to match DDI numbers on the spreadsheet with numbers from the Incoming calls <b><c>Dictionary</c></b>.</param>
        public void PopulateIncomingCalls(Dictionary<string, string> incomingCallsDictionary, int numberOfDigits)
        {
            for (int currentRowNumber = 1; currentRowNumber <= this.Sheet.LastRowNum; currentRowNumber++)
            {
                string ddiNumber = this.GetDdiNumberFromRow(this.Sheet.GetRow(currentRowNumber));

                if (ddiNumber == string.Empty)
                {
                    continue;
                }

                ICell cellE = this.Sheet.GetRow(currentRowNumber).CreateCell(4);
                ddiNumber = this.GetLastNCharacters(ddiNumber, numberOfDigits);
                this.PopulateTraffic(incomingCallsDictionary, ddiNumber, cellE);
            }
        }

        /// <summary>
        /// Gets the DDI Number from Column B of the Spreadsheet. <br />
        /// Returns <b><c>empty string</c></b> if an exception is caught.
        /// </summary>
        /// <param name="row"></param>
        /// <returns>The DDI Number in <b><c>string</c></b> format.</returns>
        public string GetDdiNumberFromRow(IRow row)
        {
            try
            {
                return row.GetCell(1).StringCellValue;
            }
            catch (Exception exception)
            {
                string errorMessage = $"Error occurred attempting to read DDI Number from row number: {row.RowNum + 1}.";
                log.Error(errorMessage);
                log.Error(exception);
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to match the DDI number provided with a value in the dictionary. <br />
        /// If a match is made, the <b><c>cell</c></b> is poulated with the <b><c>numberOfCalls</c></b>, <br />
        /// which is the value returned by the <b><c>dictionary</c></b> if a match is made. <br />
        /// If no match is made, the cell is populated with <b><c>"0"</c></b>.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="ddiNumber"></param>
        /// <param name="cell"></param>
        public void PopulateTraffic(Dictionary<string, string> dictionary, string ddiNumber, ICell cell)
        {
            string numberOfCalls;

            if (dictionary.TryGetValue(ddiNumber, out numberOfCalls))
            {
                log.Debug($"Match found for DDI Number: {ddiNumber}. Setting traffic column of row number: {cell.Row.RowNum + 1} to: {numberOfCalls}.");
                cell.SetCellValue(numberOfCalls);
            }
            else
            {
                log.Debug($"No match made for DDI Number: {ddiNumber} - setting traffic to 0 calls.");
                cell.SetCellValue("0");
            }
        }

        /// <summary>
        /// Accepts a <b><c>string</c></b>, and returns the last number of characters passed as argument. <br />
        /// Passing in <b><c>"Hello, World!"</c></b> and 3 will return <b><c>"ld!"</c></b>.
        /// </summary>
        /// <param name="str">The string to be truncated.</param>
        /// <param name="number">The number of characters to be returned.</param>
        /// <returns></returns>
        public string GetLastNCharacters(string str, int number)
        {
            return str.Substring(Math.Max(0, str.Length - number));
        }

        /// <summary>
        /// Saves the workbook locally to disc.
        /// </summary>
        public void SaveWorkbook()
        {
            using (FileStream file = new FileStream(this.FilePath, FileMode.Open, FileAccess.Write))
            {
                this.Workbook.Write(file);
                file.Close();
            }

            log.Info("Workbook successfully saved.");
        }
    }
}
