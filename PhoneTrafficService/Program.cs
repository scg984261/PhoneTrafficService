using System.Configuration;
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
        }

        public static string GetIncomingFilePathFromConfig()
        {
            return ConfigurationManager.AppSettings.Get("IncomingFilePath");
        }
    }
}
