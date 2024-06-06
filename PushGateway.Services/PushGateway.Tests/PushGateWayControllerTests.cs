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
    }
}