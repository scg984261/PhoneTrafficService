using System;
using log4net;
using NPOI.SS.UserModel;

namespace PhoneTrafficService.SpreadsheetFileHandlers
{
    public class AlbanyHouseSpreadsheetHandler : DefaultSpreadsheetHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AlbanyHouseSpreadsheetHandler));

        public AlbanyHouseSpreadsheetHandler() : base()
        {
        }

        public AlbanyHouseSpreadsheetHandler(string spreadsheetFilePath) : base(spreadsheetFilePath)
        {
            log.Debug($"Created Albany House Spreadsheet Handler with spreadsheet file path {spreadsheetFilePath}");
        }

        public override string GetDdiNumberFromRow(IRow row)
        {
            try
            {
                string ddiNumber = row.GetCell(1).StringCellValue;
                return this.GetLast7Characters(ddiNumber);
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
        /// Accepts a <b><c>string</c></b>, and returns the last 7 of characters. <br />
        /// For example, Passing in <b><c>"Hello, World!"</c></b> will return <b><c>" World!"</c></b>.
        /// </summary>
        /// <param name="str">The string to be truncated.</param>
        /// <returns></returns>
        public string GetLast7Characters(string str)
        {
            return str.Substring(Math.Max(0, str.Length - 7));
        }
    }
}
