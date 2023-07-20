using NLog;
using System.Collections.Concurrent;

namespace DeviceReadings
{
    public class Device : IProducer<DeviceMessage>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public uint Id { get; }

        public Device(uint id)
        {
            Id = id;
        }

        public void Produce(BlockingCollection<DeviceMessage> queue)
        {
            DeviceMessage deviceMessage = new DeviceMessage(Id, new Random().Next(0, 100));
            queue.Add(deviceMessage);
        }

        public Task Run(BlockingCollection<DeviceMessage> queue, CancellationToken cancellationToken)
        {
            Random random = new Random();

            Task task = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Produce(queue);
                        int delay = random.Next(1, 10) * 1000;
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error producing the message.");
                    }
                }
            }, cancellationToken);
            return task;
        }


    }
}
