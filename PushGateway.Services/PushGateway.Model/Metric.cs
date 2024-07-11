using Prometheus;

namespace PushGateway.Model
{
    public interface IReportableMetric
    {
        void Report(TimeSpan timeToLive);
    }

    public class MetricWithLabels : IReportableMetric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, string> Labels { get; set; }

        public MetricWithLabels() { }

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
            var gauge = factory.CreateGauge(Name, Description ?? string.Empty, Labels.Keys.ToArray());
            gauge.WithLease(metric => metric.Set(Value), Labels.Values.ToArray());
        }
    }

    public class MetricWithoutLabels : IReportableMetric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string? Description { get; set; }

        public void Report(TimeSpan timeToLive)
        {
            var factory = Metrics.WithManagedLifetime(expiresAfter: timeToLive);
            var gauge = factory.CreateGauge(Name, Description ?? string.Empty);
            gauge.WithLease(metric => metric.Set(metric.Value));
        }
    }
}