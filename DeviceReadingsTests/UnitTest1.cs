namespace DeviceReadingsTests
{
    [TestClass]
    public class UnitTest1
    {
        public List<IProducer<DeviceMessage>> devices = new List<IProducer<DeviceMessage>>();
        public DeviceMonitor monitor;
        public CancellationTokenSource cts;
        public CancellationTokenSource cts_main;

        [TestInitialize]
        public void Initialize()
        {
            devices = new List<IProducer<DeviceMessage>>();
            devices.Add(new Device(1));
            devices.Add(new Device(2));
            monitor = new DeviceMonitor();
            cts = new CancellationTokenSource();
            cts_main = new CancellationTokenSource();
        }
        [TestMethod]
        public void Test_Devices_Send_Messages()
        {

            monitor.Run(cts_main.Token);
            devices[0].Produce(monitor.Queue);
            devices[1].Produce(monitor.Queue);
            Thread.Sleep(1000);
            Assert.AreEqual(monitor.TotalReceived, 2);
            Assert.AreEqual(monitor.ReceivedCounts[1], 1);
            Assert.AreEqual(monitor.ReceivedCounts[2], 1);




        }
        [TestMethod]
        public void Test_Single_Device_Sends_Multiple_Messages()
        {
            monitor.Run(cts_main.Token);

            for (int i = 0; i < 50; i++)
            {
                devices[0].Produce(monitor.Queue);

            }
            Thread.Sleep(1000);
            Assert.AreEqual(monitor.TotalReceived, 50);
            Assert.AreEqual(monitor.ReceivedCounts[1], 50);
        }
        [TestMethod]
        public void Test_NoDevices_Sent_Message()
        {
            monitor.Run(cts_main.Token);
            Assert.AreEqual(monitor.TotalReceived, 0);


        }
        [TestMethod]
        public void Test_Concurrent_Message_Processing()
        {
            monitor.Run(cts_main.Token);
            for (int i = 0; i < 50; i++)
            {
                devices[0].Produce(monitor.Queue);
                devices[1].Produce(monitor.Queue);

            }
            Thread.Sleep(1000);
            Assert.AreEqual(monitor.TotalReceived, 100);
            Assert.AreEqual(monitor.ReceivedCounts[1], 50);
            Assert.AreEqual(monitor.ReceivedCounts[2], 50);

        }
        [TestMethod]
        public void Test_Large_Number_Of_Messages()
        {
            monitor.Run(cts_main.Token);
            for (int i = 0; i < 5000; i++)
            {
                devices[0].Produce(monitor.Queue);
                devices[1].Produce(monitor.Queue);

            }
            Thread.Sleep(1000);
            Assert.AreEqual(monitor.TotalReceived, 10000);
            Assert.AreEqual(monitor.ReceivedCounts[1], 5000);
            Assert.AreEqual(monitor.ReceivedCounts[2], 5000);

        }

        [TestMethod]
        public void Test_More_Active_Device()
        {
            monitor.Run(cts_main.Token);
            for (int i = 0; i < 5000; i++)
            {
                devices[0].Produce(monitor.Queue);
                if (i % 10 == 0)
                    devices[1].Produce(monitor.Queue);

            }
            Thread.Sleep(1000);
            Assert.AreEqual(monitor.TotalReceived, 5500);
            Assert.AreEqual(monitor.ReceivedCounts[1], 5000);
            Assert.AreEqual(monitor.ReceivedCounts[2], 500);

        }



    }
}