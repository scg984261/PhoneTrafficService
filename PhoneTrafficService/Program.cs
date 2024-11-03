using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace PhoneTrafficService
{

    /*
     * TODO:
     * 
     * Set the header of Phone Numbers Allocated if it's not set already.
     * Testing
     * Logging
     * Different format of INCOMING.csv
     * Installer package - how do we get this thing onto the server?
     * MSI Installer?
     */
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static string IncomingFileLocation { get; set; } = ConfigurationManager.AppSettings.Get("IncomingFilePath");
        public static string PhoneNumbersAllocated { get; set; } = ConfigurationManager.AppSettings.Get("PhoneNumbersAllocatedSpreadsheet");

        static Program()
        {
            XmlConfigurator.Configure();
        }

        public static void Main(string[] args)
        {   
            log.Info("Start of Application.");
            log.Info($"File location read from config: {IncomingFileLocation}");

            string[] lines = File.ReadAllLines(IncomingFileLocation).Skip(1).ToArray();

            string ddiNumber;
            string numberOfCalls;
            int numberOfCallsInt;
            Dictionary<string, string> incomingCallsDictionary = new Dictionary<string, string>();

            foreach (string line in lines)
            {
                // N.B. It seems there are two formats of INCOMING.CSV now - with one having total number of calls in one column,
                // and the old one had incoming and outgoing calls in separate columns.
                string[] columns = line.Split(',');
                ddiNumber = columns[0];
                numberOfCalls = columns[1];

                if (int.TryParse(numberOfCalls, out numberOfCallsInt))
                {
                    incomingCallsDictionary.Add(ddiNumber, numberOfCallsInt.ToString());
                }
                else
                {
                    log.Error($"Error occurred reading number of calls: {numberOfCalls} is not an integer!");
                    incomingCallsDictionary.Add(ddiNumber, "0");
                }
            }

            string spreadsheetFileName = PhoneNumbersAllocated;

            HSSFWorkbook hssfwb;

            using (FileStream fileStream = new FileStream(spreadsheetFileName, FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(fileStream);
                fileStream.Close();
            }

            ISheet sheet = hssfwb.GetSheetAt(0);

            IRow headerRow = sheet.GetRow(0);
            ICell trafficHeaderCell = headerRow.GetCell(4);

            if (trafficHeaderCell.StringCellValue != "Traffic")
            {
                trafficHeaderCell.SetCellValue("Traffic");
            }

            int lastRowNumber = sheet.LastRowNum;

            // Start from cell 2, not top row.
            for (int i = 1; i < lastRowNumber; i++)
            {
                IRow row = sheet.GetRow(i);
                ICell cellE = row.CreateCell(4);

                string phoneNumbersAllocatedDdiNumber;

                try
                {
                    phoneNumbersAllocatedDdiNumber = row.GetCell(1).StringCellValue;
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    continue;
                }

                string phoneNumbersAllocatedDdiNumberLastTenDigits = phoneNumbersAllocatedDdiNumber.Substring(Math.Max(0, phoneNumbersAllocatedDdiNumber.Length - 10));

                string numberOfCallsNew;

                if (incomingCallsDictionary.TryGetValue(phoneNumbersAllocatedDdiNumberLastTenDigits, out numberOfCallsNew))
                {
                    cellE.SetCellValue(numberOfCallsNew);
                }
                else if (phoneNumbersAllocatedDdiNumberLastTenDigits.Length > 0)
                {
                    cellE.SetCellValue("0");
                }
                else
                {
                    continue;
                }
            }

            using (FileStream file = new FileStream(spreadsheetFileName, FileMode.Open, FileAccess.Write))
            {
                hssfwb.Write(file);
                file.Close();
            }
        }
    }
}
