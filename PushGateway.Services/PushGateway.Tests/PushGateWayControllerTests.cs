using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public MetricRequestDto metricsDto;
        public MetricRequestListDto metricsListDto;
        public string stringMetric;
        public Mock<IMetricService> metricService;
        public Mock<ILogger<MetricController>> logger;

        [TestInitialize]
        public void SetUp()
        {
            metricsDto = new MetricRequestDto()
            {
                Description = "description",
                Labels = new Dictionary<string, string>(),
                Name = "name",
                Value = 0
            };
            metricsDto.Labels.Add("name", "value");

            metricsListDto = new MetricRequestListDto()
            {
                Metrics = new List<MetricRequestDto> { metricsDto }
            };

            stringMetric = "string{additionalProp1=\"string1\",additionalProp2=\"string\",additionalProp3=\"string\"} 0";

            metricService = new Mock<IMetricService>();
            logger = new Mock<ILogger<MetricController>>();
        }

        [TestMethod]
        public void ReportMetricDto_Success()
        {
            //Arrange
            metricService.Setup(x => x.AddListMetricsDto(metricsListDto));
            var controller = new MetricController(logger.Object, metricService.Object);

            //Act

            var expectedResult = controller.ReportMetricDto(metricsListDto);

            //Assert
            metricService.VerifyAll();
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as OkResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 200);
        }

        [TestMethod]
        public void ReportMetricString_InvalidMetric_BadRequests()
        {
            //Arrange
            var validationErrors = "Metric is invalid";
            metricService.Setup(x => x.ReportStringMetrics(stringMetric, out validationErrors));
            var controller = new MetricController(logger.Object, metricService.Object);

            //Act

            var expectedResult = controller.ReportMetricString(stringMetric);

            //Assert
            metricService.VerifyAll();
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 400);
            Assert.AreEqual(actionResult.Value, validationErrors);
        }

        [TestMethod]
        public void ReportMetricString_Success()
        {
            //Arrange
            string validationErrors = null;
            metricService.Setup(x => x.ReportStringMetrics(stringMetric, out validationErrors));
            var controller = new MetricController(logger.Object, metricService.Object);

            //Act

            var expectedResult = controller.ReportMetricString(stringMetric);

            //Assert
            metricService.VerifyAll();
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as OkResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 200);
        }
    }
}