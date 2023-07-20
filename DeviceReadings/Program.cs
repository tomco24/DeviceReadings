using DeviceReadings;
using NLog;

internal class Program
{
    private static void Main(string[] args)
    {
        //Log setup
        NLog.LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToFile(fileName: "app.log");
        });

        // Device list initialization
        List<IProducer<DeviceMessage>> devices = new List<IProducer<DeviceMessage>>();
        devices.Add(new Device(1));
        devices.Add(new Device(2));

        DeviceMonitor monitor = new DeviceMonitor();
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationTokenSource cts_main = new CancellationTokenSource();

        Task mainTask = monitor.Run(cts_main.Token);
        foreach (var device in devices)
        {
            device.Run(monitor.Queue, cts.Token);
        }
        mainTask.Wait();
    }
}