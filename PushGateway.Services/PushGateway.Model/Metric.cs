using Prometheus;

namespace PushGateway.Model
{
    public interface IReportableMetric
    {
        void Report(TimeSpan timeToLive);
    }

    public class MetricWithLabels : IReportableMetric
    {
        public string Name { get; init; }
        public double Value { get; init; }
        public string? Description { get; init; }
        public Dictionary<string, string> Labels { get; init; }

        public MetricWithLabels()
        {
        }

        public MetricWithLabels(MetricRequestDto metricDto)
        {
            Name = metricDto.Name;
            Value = metricDto.Value;
            Labels = metricDto.Labels;
            Description = metricDto.Description;
        }

        public void Report(TimeSpan timeToLive)
        {
            var factory = Metrics.WithManagedLifetime(expiresAfter: timeToLive);
            var labelNames = Labels.Keys.ToArray();
            var labelValues = Labels.Values.ToArray();
            var gauge = factory.CreateGauge(Name, Description ?? string.Empty, labelNames);
            gauge.WithLease(m => m.Set(Value), labelValues);
        }
    }

    public class MetricWithoutLabels : IReportableMetric
    {
        public string Name { get; init; }
        public double Value { get; init; }
        public string? Description { get; init; }

        public void Report(TimeSpan timeToLive)
        {
            var factory = Metrics.WithManagedLifetime(expiresAfter: timeToLive);
            var gauge = factory.CreateGauge(Name, Description ?? string.Empty);
            gauge.WithLease(m => m.Set(Value));
        }
    }
}