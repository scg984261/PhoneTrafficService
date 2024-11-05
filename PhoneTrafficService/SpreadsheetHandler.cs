using System;
using log4net;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Collections.Generic;

namespace PhoneTrafficService
{
    public class SpreadsheetHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SpreadsheetHandler));

        public string FilePath { get; set; }
        public HSSFWorkbook Workbook { get; set; }
        public ISheet Sheet { get; set; }

        public SpreadsheetHandler()
        {
        }

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

        public void PopulateIncomingCalls(Dictionary<string, string> incomingCallsDictionary)
        {
            for (int currentRowNumber = 1; currentRowNumber <= this.Sheet.LastRowNum; currentRowNumber++)
            {
                string ddiNumber = this.GetDdiNumberFromRow(this.Sheet.GetRow(currentRowNumber));

                if (ddiNumber == string.Empty)
                {
                    continue;
                }

                ICell cellE = this.Sheet.GetRow(currentRowNumber).CreateCell(4);
                ddiNumber = this.GetLastTenDigits(ddiNumber);
                this.PopulateTraffic(incomingCallsDictionary, ddiNumber, cellE);
            }
        }

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

        public void PopulateTraffic(Dictionary<string, string> dictionary, string ddiNumber, ICell cell)
        {
            string numberOfCalls;

            if (dictionary.TryGetValue(ddiNumber, out numberOfCalls))
            {
                log.Debug($"Setting traffic column of row number: {cell.Row.RowNum + 1} to: {numberOfCalls}.");
                cell.SetCellValue(numberOfCalls);
            }
            else
            {
                log.Debug($"No match made for DDI Number: {ddiNumber} - setting traffic to 0 calls.");
                cell.SetCellValue("0");
            }
        }

        public string GetLastTenDigits(string str)
        {
            return str.Substring(Math.Max(0, str.Length - 10));
        }

        public void SaveWorkbook()
        {
            using (FileStream file = new FileStream(this.FilePath, FileMode.Open, FileAccess.Write))
            {
                this.Workbook.Write(file);
                file.Close();
            }
        }
    }
}
