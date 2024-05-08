using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushGateway.Model
{
    public interface IMetricService
    {
        void AddListMetrics(MetricRequestListDto metricsDto);
        void AddMetrics(MetricRequestDto metricDto);
        MetricRequestDto MetricParse(string metric);
        MetricRequestListDto MetricsListParse(string metrics);
    }
}
