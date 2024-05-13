using Microsoft.AspNetCore.Mvc;
using PushGateway.Model;
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

        [HttpPost("dto")]
        public async Task<IActionResult> PostMetricsList([FromBody] MetricRequestListDto metricsDto)
        {
            _metricService.AddListMetricsDto(metricsDto);
            return Ok();
        }

        [HttpPost("string")]
        public async Task<IActionResult> PostMetricsString([FromQuery] string metricsList)
        {
            await _metricService.ParseListStringMetrics(metricsList);
            return Ok();
        }
    }
}
