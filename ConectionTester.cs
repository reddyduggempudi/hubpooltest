using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace MyApp
{
    public class ConnectionTester
    {

        public ConnectionTester()
        {
        }

        public async Task PerformTest(int numDevices)
        {
            string fileName = "devices.txt";
            string[] lines = File.ReadAllLines(fileName);
            if (numDevices > lines.Length)
            {
                Log($"Expecting to simulate {numDevices}, but found only {lines.Length} devices in {fileName}");
            }
            else
            {
                SampleDevice[] devices = new SampleDevice[numDevices];

                // connect all devices
                Log($"Connecting to {numDevices}...");
                Task[] connectTasks = new Task[numDevices];
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < numDevices; i++)
                {
                    devices[i] = new SampleDevice() { DeviceNum = i + 1 };
                    devices[i].ConnectionString = lines[i];
                    connectTasks[i] = devices[i].Connect();
                }
                Task.WaitAll(connectTasks);
                sw.Stop();
                Log($"Connected to {numDevices} in {sw.ElapsedMilliseconds} ms");

                // Send telemetry N number of times
                int numMessages = 5;
                for (int msgNum = 0; msgNum < numMessages; msgNum++)
                {
                    sw.Reset();
                    sw.Start();

                    // Send telemetry from all devices
                    Task[] tasks = new Task[numDevices];
                    for (int i = 0; i < numDevices; i++)
                    {
                        devices[i].ConnectionString = lines[i];
                        tasks[i] = devices[i].SendMessagesAsync();
                    }

                    Task.WaitAll(tasks);
                    sw.Stop();
                    Log($"Sent telemetry to {numDevices} in {sw.ElapsedMilliseconds} ms");

                    if (msgNum != numMessages-1)
                    {
                        await Task.Delay(30000);
                    }
                }
            }
        }

        private void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now}: {msg}");
        }
    }
}
