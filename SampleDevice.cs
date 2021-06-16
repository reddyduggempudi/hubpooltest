using Microsoft.Azure.Devices.Client;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace MyApp
{
    /// <summary>
    /// This sample illustrates the very basics of a device app sending telemetry over AMQP protocol. 
    /// </summary>
    public class SampleDevice
    {
        private DeviceClient deviceClient;

        public int DeviceNum { get; set; }
        public string ConnectionString { get; set; }

        public SampleDevice()
        {
        }

        public async Task Connect()
        {
            var transportSettings = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only)
            {
                AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings
                {
                    // enable connection pooling
                    Pooling = true,
                    // connections are multiplexed from this program to IoT Hub.
                    // configure how many number of physical connections to open to IoT Hub.
                    // E.g.: If there are 1000 devices connecting from this program, you can have all of them use only 100 physical connections
                    MaxPoolSize = 100
                }
            };

            // Connect to the IoT hub using the AMQP protocol
            deviceClient = DeviceClient.CreateFromConnectionString(this.ConnectionString, new[] { transportSettings });
            await deviceClient.OpenAsync();
        }

        public async Task Close()
        {
            await deviceClient.CloseAsync();

            deviceClient.Dispose();
            Console.WriteLine("Device simulator finished.");
        }

        // Async method to send simulated telemetry
        public async Task SendMessagesAsync()
        {
            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            var rand = new Random();

            double currentTemperature = minTemperature + rand.NextDouble() * 15;
            double currentHumidity = minHumidity + rand.NextDouble() * 20;

            // Create JSON message
            string messageBody = JsonSerializer.Serialize(
                new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity,
                });
            using var message = new Message(Encoding.ASCII.GetBytes(messageBody))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8",
            };

            // Send the telemetry message
            await deviceClient.SendEventAsync(message);
            //Console.WriteLine($"{DateTime.Now} > Sending message from device {DeviceNum}: {messageBody}");
        }
    }
}