using System.Collections.Concurrent;

namespace DeviceReadings
{
    public interface IProducer<T>
    {
        public void Produce(BlockingCollection<T> queue);
        public Task Run(BlockingCollection<T> queue, CancellationToken cancellationToken);
    }
}
