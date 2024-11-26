using System.Collections.Generic;

namespace PhoneTrafficService.CsvFileProcessors
{
    public interface ICsvFileProcessor
    {
        string[] ReadLinesFromFile();
        void PopulateIncomingCalls(Dictionary<string, string> dictionary, string[] lines);
        void ProcessCsvLine(Dictionary<string, string> dictionary, string lines);
    }
}
