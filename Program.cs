using System;
using System.Threading.Tasks;

namespace MyApp
{
    public class Program
    {
        static void Main(string[] args)
        {

            if ((args[0] != "-a" && args[0] != "-c")
                || (args[0] == "-a" && args.Length != 3)
                || (args[0] == "-c" && args.Length != 2))
            {
                Usage();
            }
            else if (args[0] == "-a")
            {
                int numDevices = int.Parse(args[1]);
                string hubConnectionString = args[2];
                DeviceCreator creator = new DeviceCreator();
                creator.Initialize(hubConnectionString);
                creator.CreateDevices(numDevices).Wait();
            } else {
                int numDevices = int.Parse(args[1]);
                ConnectionTester conTester = new ConnectionTester();
                conTester.PerformTest(numDevices).Wait();
            }
        }

        public static void Usage()
        {
            Console.WriteLine("Missing or invalid commandline arguments");
            Console.WriteLine("");
            Console.WriteLine("Usage: HubPoolTest -a <numDevices> <hubConnectionString");
            Console.WriteLine("or     HubPoolTest -c <numDevices>");
            Console.WriteLine("E.g.:");
            Console.WriteLine("   To add 1000 devices use: HubPoolTest -a 1000 YOUR_IOT_HUB_CONNECTIONSTRING");
            Console.WriteLine("   To do 500 device connection test use: HubPoolTest -c 500");
        }
    }
}
