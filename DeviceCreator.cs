using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;

namespace MyApp
{
    public class DeviceCreator
    {

        private RegistryManager registryManager;
        private string hubConnectionString;

        public DeviceCreator()
        {
        }

        public void Initialize(string hubConnectionString)
        {
            this.hubConnectionString = hubConnectionString;
            registryManager = RegistryManager.CreateFromConnectionString(hubConnectionString);
        }

        public async Task CreateDevices(int numDevices)
        {
            string hostName = hubConnectionString.Split(';')[0];
            string fileName = "devices.txt";
            using (StreamWriter devicesFile = new StreamWriter(fileName))
            {
                for (int i = 0; i < numDevices; i++)
                {
                    string deviceId = "Device." + string.Format("{0:000000000}", i + 1);
                    string primaryKey = CryptoKeyGenerator.GenerateKey(32);
                    string secondaryKey = CryptoKeyGenerator.GenerateKey(32);
                    Device device = new Device(deviceId)
                    {
                        Authentication = new AuthenticationMechanism
                        {
                            Type = AuthenticationType.Sas,
                            SymmetricKey = {
                            PrimaryKey = primaryKey,
                            SecondaryKey = secondaryKey
                        }
                        },
                    };

                    try
                    {
                        if (i % 10 == 0)
                        {
                            Console.WriteLine($"Creating device # {i + 1}...");
                        }

                        // create new device in IoT Hub
                        await registryManager.AddDeviceAsync(device);

                        // add the device to the devices file
                        devicesFile.WriteLine(hostName + ";DeviceId=" + deviceId + ";SharedAccessKey=" + primaryKey);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to add devices, error: {ex.Message}.");
                    }
                }
            }

            Console.WriteLine($"Added {numDevices} devices to IoT Hub and wrote them to {fileName}");
        }
    }
}