using System.Collections.Concurrent;

namespace DeviceReadings
{
    public class DeviceMonitor
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// The queue of device messages.        
        private BlockingCollection<DeviceMessage> _queue;

       
        // A dictionary of the number of messages received from each device.
        private Dictionary<uint, int> _receivedCounts;

        
        private int _totalReceived;
        public int TotalReceived { get => _totalReceived; }
        public BlockingCollection<DeviceMessage> Queue { get => _queue; }
        public Dictionary<uint, int> ReceivedCounts { get => _receivedCounts; }

        public DeviceMonitor()
        {
            _totalReceived = 0;
            _queue = new BlockingCollection<DeviceMessage>();
            _receivedCounts = new Dictionary<uint, int>();

        }
        /// Starts a task that monitors the queue.
        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        DeviceMessage message = _queue.Take();
                        if (_receivedCounts.ContainsKey(message.DeviceId))
                        {
                            _receivedCounts[message.DeviceId] += 1;
                        }
                        else
                        {
                            _receivedCounts.Add(message.DeviceId, 1);
                        }
                        _totalReceived++;
                        Logger.Info($"Received: {message}");
                        Logger.Info($"Received: {_receivedCounts[message.DeviceId]} messages from device with ID: {message.DeviceId}");
                        Logger.Info($"Total messages received: {_totalReceived}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error processing message.");
                    }
                }
            });
        }

    }
}
