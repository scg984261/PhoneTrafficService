using System.Configuration;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;

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

            foreach (string line in lines)
            {
                // N.B. It seems there are two formats of INCOMING.CSV now - with one having total number of calls in one column,
                // and the old one had incoming and outgoing calls in separate columns.
                string[] columns = line.Split(',');
                ddiNumber = columns[0];
                numberOfCalls = columns[1];
            }
        }

        public static string GetIncomingFilePathFromConfig()
        {
            return ConfigurationManager.AppSettings.Get("IncomingFilePath");
        }

        // public static string[] GetFileContentsSkipFirstLine(string fileLocation)
        // {
        //     return File.ReadAllLines(fileLocation).Skip(1).ToArray();
        // }
    }
}
