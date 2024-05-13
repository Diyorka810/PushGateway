using Microsoft.Extensions.Configuration;
using Prometheus;
using PushGateway.Model;
using System.Configuration;
using System.Reflection.PortableExecutable;

namespace PushGateway.Model
{
    public class MetricService : IMetricService
    {
        public TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(10);
        //private readonly IConfiguration Configuration;
        //public MetricsOptions positionOptions { get; private set; }

        //public MetricService(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //    var metricOptions = new MetricsOptions();
        //    Configuration.GetSection(MetricsOptions.Position).Bind(metricOptions);
        //    TimeToLive = metricOptions.Minutes;
        //}

        public void AddMetrics(Metric metric)
        {
            var factory = Metrics.WithManagedLifetime(expiresAfter: TimeToLive);
            if (metric.Labels != null)
            {
                var gauge = factory.CreateGauge(metric.Name, metric.Description ?? string.Empty, metric.Labels.Keys.ToArray() ?? new string[0]);
                gauge.WithLease(metric => metric.Set(metric.Value), metric.Labels.Values.ToArray());
            }
            else
            {
                var gauge = factory.CreateGauge(metric.Name, metric.Description ?? string.Empty);
                gauge.WithLease(metric => metric.Set(metric.Value));
            }
        }

        public void AddListMetricsDto(MetricRequestListDto metricsDto)
        {
            foreach (var metricDto in metricsDto.Metrics)
            {
                var metric = new Metric(metricDto);
                AddMetrics(metric);
            }
        }

        public async Task<List<string>> ReadFile(FileStream metricsFile)
        {
            var steamReader = new StreamReader(metricsFile);
            var list = new List<string>();
            string? line;
            while ((line = await steamReader.ReadLineAsync()) != null)
            {
                list.Add(line);
            }
            return list;
        }

        public async Task ParseListStringMetrics(string metricsList)
        {
            var splitMetricsList = metricsList.Split("\\n");
            string description = "";
            foreach (var line in splitMetricsList)
            {
                var splitedLine = line.Split(" ");
                if (splitedLine[0] == "#" && splitedLine[1] == "HELP")
                {
                    var help = splitedLine.Skip(3);
                    description = help.Aggregate((x, y) => x + y);
                }
                else if((splitedLine[0] == "#" && splitedLine[1] == "TYPE") || line == string.Empty)
                {
                    continue;
                } 
                else
                {
                    var metric = new Metric(line, description);
                    AddMetrics(metric);
                }

            }
        }
    }
}
