using PushGateway.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushGateway.Model
{
    public interface IMetricService
    {
        void AddListMetricsDto(MetricRequestListDto metricsDto);
        void ReportStringMetrics(string metricsList, out string? validationErrors);
    }
}
