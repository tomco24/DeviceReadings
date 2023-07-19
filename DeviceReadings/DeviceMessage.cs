namespace DeviceReadings
{
    public record DeviceMessage
    {
        public uint DeviceId { get; init; }
        public int Value { get; init; }

        public DeviceMessage(uint deviceId, int value)
        {
            DeviceId = deviceId;
            Value = value;
        }
    }
}
