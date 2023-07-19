using DeviceReadings;
using NLog;

internal class Program
{
    private static void Main(string[] args)
    {
        NLog.LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToFile(fileName: "app.log");
        });
        List<IProducer<DeviceMessage>> devices = new List<IProducer<DeviceMessage>>();
        devices.Add(new Device(1));
        devices.Add(new Device(2));
        DeviceMonitor reciever = new DeviceMonitor();
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationTokenSource cts_main = new CancellationTokenSource();

        Task mainTask = reciever.Run(cts_main.Token);
        foreach (var device in devices)
        {
            device.Run(reciever.Queue, cts.Token);
        }
        //reciever.Run();
        mainTask.Wait();
    }
}