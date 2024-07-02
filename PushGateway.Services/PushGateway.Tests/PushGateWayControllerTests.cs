using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using PushGateway.Model;
using PushGateway.Services;
using PushGateway.Services.Controllers;

namespace PushGateway.Tests
{
    [TestClass]
    public class PushGateWayControllerTests
    {
        [TestMethod]
        public void ReportMetricDto()
        {
            //Arrange
            var metricsDto = new MetricRequestDto()
            {
                Description = "description",
                Labels = new Dictionary<string, string>(),
                Name = "name",
                Value = 0
            };
            metricsDto.Labels.Add("name", "value");
            var metricsListDto = new MetricRequestListDto()
            {
                Metrics = new List<MetricRequestDto> { metricsDto }
            };
            var metricService = new Mock<IMetricService>();
            metricService.Setup(x => x.AddListMetricsDto(metricsListDto));
            var logger = new Mock<ILogger<MetricController>>();
            var controller = new MetricController(logger.Object, metricService.Object);

            //Act

            controller.ReportMetricDto(metricsListDto);

            //Assert
            metricService.VerifyAll();
        }

        [TestMethod]
        public void LOL()
        {
            var height = new int[] { 8, 20, 1, 2, 3, 4, 5, 6 };

            int start = 0;
            int end = height.Length - 1;
            int fIndex = 0;
            int fValue = 0;

            while (start != height.Length)
            {
                if (fValue < height[start] * (height.Length - start - 1))
                {
                    fIndex = start;
                    fValue = height[start] * (height.Length - start - 1);
                }
                start++;
            }
            var res = 0;
            for (int i = fIndex; i < height.Length; i++)
            {
                var w = i - fIndex;
                var h = height[fIndex] < height[i] ? height[fIndex] : height[i];
                if (res < w * h)
                {
                    res = w * h;
                }
            }


            Assert.AreEqual(res, 202);
        }
    }
}