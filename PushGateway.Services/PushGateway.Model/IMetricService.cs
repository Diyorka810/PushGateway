namespace PushGateway.Model
{
    public interface IMetricService
    {
        void AddListMetricsDto(MetricRequestListDto metricsDto);
        void ReportStringMetrics(string metricsList, out string? validationErrors);
    }
}