using Microsoft.Extensions.Configuration;
using Prometheus;
using System.Configuration;

namespace PushGateway.Model
{
    public class MetricService : IMetricService
    {
        public TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(10);
        private readonly IConfiguration Configuration;
        public MetricsOptions positionOptions { get; private set; }

        public MetricService(IConfiguration configuration)
        {
            Configuration = configuration;
            var metricOptions = new MetricsOptions();
            Configuration.GetSection(MetricsOptions.Position).Bind(metricOptions);
            TimeToLive = metricOptions.Minutes;
        }

        public void AddMetrics(MetricRequestDto metricDto)
        {
            var factory = Metrics.WithManagedLifetime(expiresAfter: TimeToLive);
            var gauge = factory.CreateGauge(metricDto.Name, metricDto.Description ?? string.Empty, metricDto.Labels.Keys.ToArray());
            gauge.WithLease(metric=> metric.Set(metricDto.Value), metricDto.Labels.Values.ToArray());
        }

        public void AddListMetrics(MetricRequestListDto metricsDto)
        {
            foreach (var metric in metricsDto.Metrics)
            {
                AddMetrics(metric);
            }
        }

        public MetricRequestDto MetricParse(string metric)
        {
            var metricDto = new MetricRequestDto();

            var splittedMetricName = metric.Split("{");
            metricDto.Name = splittedMetricName[0];
            var splittedMetricValue= splittedMetricName[1].Split("}");
            metricDto.Value = double.Parse(splittedMetricValue[1].Trim());

            var splittedMetricLabels = splittedMetricValue[0].Split(",");
            var dict = new Dictionary<string, string>();
            var chars = new Char[] { '\\', '\"' };
            foreach (var item in splittedMetricLabels)
            {
                var label = item.Split("=");
                dict.Add(label[0].Trim(chars), label[1].Trim(chars));
            }
            metricDto.Labels = dict;


            return metricDto;
        }

        public MetricRequestListDto MetricsListParse(string metrics)
        {
            var metricList = new MetricRequestListDto();
            foreach (var metric in metrics.Split("\n"))
            {
                metricList.Metrics.Add(MetricParse(metric));
            }
            return metricList;
        }
    }
}
