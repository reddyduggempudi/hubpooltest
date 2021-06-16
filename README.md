# hubpooltest
IoT Hub AMQP Pooling test

Install .NET 5.0 SDK
Open this folder from VS Code and run it.

1. Create devices using `HubPoolTest -a <numDevices> <YOUR_IOT_HUB_CONNECTION_STRING>`
2. Send telemetry from devices using `HubPoolTest -c <numDevices>`

You can change the MaxPoolSize in SampleDevice.cs to adjust the number of connection sopen from this program to the IoT Hub.
These will be shared between all  device clients.

The following command shows the number of open ports. You can use it before you run the program and during the program is running.

`netstat -a | grep TCP | wc -l`
