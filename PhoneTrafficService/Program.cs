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
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static string incomingFileLocation = GetIncomingFilePathFromConfig();

        static Program()
        {
            XmlConfigurator.Configure();
        }

        public static void Main(string[] args)
        {   
            log.Info("Start of Application.");
            log.Info($"File location read from config: {incomingFileLocation}");

            string[] lines = File.ReadAllLines(incomingFileLocation).Skip(1).ToArray();

            string ddiNumber;
            string numberOfCalls;
            int numberOfCallsInt;
            Dictionary<string, int> incomingCallsDictionary = new Dictionary<string, int>();

            foreach (string line in lines)
            {
                // N.B. It seems there are two formats of INCOMING.CSV now - with one having total number of calls in one column,
                // and the old one had incoming and outgoing calls in separate columns.
                string[] columns = line.Split(',');
                ddiNumber = columns[0];
                numberOfCalls = columns[1];

                if (int.TryParse(numberOfCalls, out numberOfCallsInt))
                {
                    incomingCallsDictionary.Add(ddiNumber, numberOfCallsInt);
                }
                else
                {
                    log.Error($"Error occurred attempting to convert number of calls: {numberOfCalls} to an integer type!");
                    incomingCallsDictionary.Add(ddiNumber, 0);
                }
            }

            string spreadsheetFileName = ConfigurationManager.AppSettings.Get("PhoneNumbersAllocatedSpreadsheet");

            HSSFWorkbook hssfwb;

            using (FileStream fileStream = new FileStream(spreadsheetFileName, FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(fileStream);
                fileStream.Close();
            }

            ISheet sheet = hssfwb.GetSheetAt(0);

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

                int numberOfCallsNew;

                if (incomingCallsDictionary.TryGetValue(phoneNumbersAllocatedDdiNumberLastTenDigits, out numberOfCallsNew))
                {
                    cellE.SetCellValue(numberOfCallsNew);
                }
                else if (phoneNumbersAllocatedDdiNumberLastTenDigits.Length > 0)
                {
                    cellE.SetCellValue(0);
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

        public static string GetIncomingFilePathFromConfig()
        {
            return ConfigurationManager.AppSettings.Get("IncomingFilePath");
        }
    }
}
