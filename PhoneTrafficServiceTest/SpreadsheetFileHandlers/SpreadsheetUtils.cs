using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneTrafficServiceTest.SpreadsheetFileHandlers
{
    public class SpreadsheetUtils
    {
        public static HSSFWorkbook CreateTestWorkbook(string[] testSpreadSheetValues)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet();

            for (int i = 0; i < testSpreadSheetValues.Length; i++)
            {
                IRow row = sheet.CreateRow(i);
                row.CreateCell(0);
                row.CreateCell(1).SetCellValue(testSpreadSheetValues[i]);
                row.CreateCell(2);
                row.CreateCell(3);
            }

           return workbook;
        }
    }
}
