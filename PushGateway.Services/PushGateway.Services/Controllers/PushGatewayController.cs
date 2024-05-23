using Microsoft.AspNetCore.Mvc;
using PushGateway.Model;

namespace PushGateway.Services.Controllers
{
    [ApiController]
    [Route("push/metrics")]
    public class MetricController : ControllerBase
    {
        private readonly ILogger<MetricController> _logger;
        private IMetricService _metricService;

        public MetricController(ILogger<MetricController> logger, IMetricService metricService)
        {
            _logger = logger;
            _metricService = metricService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ReportMetricDto([FromBody] MetricRequestListDto metricsDto)
        {
            _metricService.AddListMetricsDto(metricsDto);
            return Ok();
        }

        [HttpPost("stringList")]
        public async Task<IActionResult> ReportMetricString([FromQuery] string metricsList)
        {
            _metricService.ReportStringMetrics(metricsList, out string validationErrors);
            if (validationErrors != null)
                return StatusCode(StatusCodes.Status400BadRequest, validationErrors);

            return Ok();
        }
    }
}
