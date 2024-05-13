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
        Task ParseListStringMetrics(string metricsList);
    }
}
