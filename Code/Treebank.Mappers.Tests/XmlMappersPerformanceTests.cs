namespace Treebank.Mappers.Tests
{
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;
    using Events;
    using LightWeight;
    using NSubstitute;
    using Prism.Events;
    using Serialization.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class XmlMappersPerformanceTests
    {
        private readonly ITestOutputHelper output;

        public XmlMappersPerformanceTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(StaticData.LargeXmlinputFilepath, StaticData.XmlConfigFilepath)]
        public void LightDocumentMapperWithReaderTest(string inputFilepath, string configFilepath)
        {
            var eventAggregatorMock = Substitute.For<IEventAggregator>();
            eventAggregatorMock.GetEvent<ValidationExceptionEvent>().Returns(new ValidationExceptionEvent());
            eventAggregatorMock.GetEvent<StatusNotificationEvent>().Returns(new StatusNotificationEvent());
            var sut = new LightDocumentMapperWithReader
            {
                AppConfigMapper = new AppConfigMapper
                {
                    EventAggregator = eventAggregatorMock
                },
                EventAggregator = eventAggregatorMock
            };

            var timer = new Stopwatch();
            timer.Start();
            var doc = sut.Map(inputFilepath, configFilepath).GetAwaiter().GetResult();
            timer.Stop();

            output.WriteLine(
                $"Time to load: {timer.ElapsedMilliseconds} ms,\r\n\tType: {sut.GetType().FullName},\r\n\tTest File: {inputFilepath},\r\n\tConfig File: {configFilepath}\r\n");
        }

        [Theory]
        [InlineData(StaticData.LargeXmlinputFilepath, StaticData.XmlConfigFilepath)]
        public void LightDocumentMapperWithXmlReaderTest(string inputFilepath, string configFilepath)
        {
            var eventAggregatorMock = Substitute.For<IEventAggregator>();
            eventAggregatorMock.GetEvent<ValidationExceptionEvent>().Returns(new ValidationExceptionEvent());
            eventAggregatorMock.GetEvent<StatusNotificationEvent>().Returns(new StatusNotificationEvent());
            var sut = new LightDocumentMapperWithXmlReader
            {
                AppConfigMapper = new AppConfigMapper
                {
                    EventAggregator = eventAggregatorMock
                },
                EventAggregator = eventAggregatorMock
            };

            var timer = new Stopwatch();
            timer.Start();
            var doc = sut.Map(inputFilepath, configFilepath).GetAwaiter().GetResult();
            timer.Stop();

            output.WriteLine(
                $"Time to load: {timer.ElapsedMilliseconds} ms,\r\n\tType: {sut.GetType().FullName},\r\n\tTest File: {inputFilepath},\r\n\tConfig File: {configFilepath}\r\n");
        }

        [Theory]
        [InlineData(StaticData.LargeXmlinputFilepath)]
        public void DeserializationTest(string inputFilepath)
        {
            var ser = new XmlSerializer(typeof(Treebank));
            var timer = new Stopwatch();

            timer.Start();
            var treebank = ser.Deserialize(XmlReader.Create(inputFilepath));
            timer.Stop();

            output.WriteLine(
                $"Time to deserialize: {timer.ElapsedMilliseconds} ms,\r\n\tType: {ser.GetType().FullName},\r\n\tTest File: {inputFilepath}");
        }
    }
}