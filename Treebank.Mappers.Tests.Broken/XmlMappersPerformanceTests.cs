namespace Treebank.Mappers.Tests
{
    using System;
    using System.Diagnostics;
    using Treebank.Mappers.LightWeight;
    using Xunit;

    public class XmlMappersPerformanceTests
    {
        [Theory]
        [InlineData(StaticData.LargeConlluInputFilepath)]
        public void LightDocumentMapperWithReaderTest(string inputFilepath)
        {
            var sut = new LightDocumentMapperWithReader();

            var timer = new Stopwatch();
             timer.Start();
            var doc = sut.Map(StaticData.ConlluConfigFilepath, inputFilepath).GetAwaiter().GetResult();
            timer.Stop();

            Console.Out.WriteLine($"Time to load: {timer.ElapsedMilliseconds} ms");
        }
    }
}