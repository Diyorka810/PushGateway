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

        //[HttpPost("single")]
        //public async Task<IActionResult> Post([FromBody] MetricRequestDto metricDto)
        //{
        //    _metricService.LOL(metricDto);
        //    return Ok();
        //}

        [HttpPost("list")]
        public async Task<IActionResult> PostMetricsList([FromBody] MetricRequestListDto metricsDto)
        {
            _metricService.AddListMetrics(metricsDto);
            return Ok();
        }

        [HttpPost("string")]
        public async Task<IActionResult> PostStringMetric([FromQuery] string metric)
        {
            var metricDto = _metricService.MetricParse(metric);
            _metricService.AddMetrics(metricDto);

            return Ok();
        }

        [HttpPost("stringList")]
        public async Task<IActionResult> PostStringMetricList([FromQuery] string metricsList)
        {
            //игнорировать решетки
            var metricListDto = _metricService.MetricsListParse(metricsList);
            _metricService.AddListMetrics(metricListDto);

            return Ok();
        }
    }
}
