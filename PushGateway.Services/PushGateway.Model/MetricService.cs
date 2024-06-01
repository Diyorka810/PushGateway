using Microsoft.Extensions.Configuration;
using Prometheus;
using PushGateway.Model;
using System.Configuration;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using System.Reflection.Emit;

namespace PushGateway.Model
{
    public class MetricService : IMetricService
    {
        private readonly TimeSpan _timeToLive;
        private const string Name = "^(\\w+)";
        private const string Labels = "(?:\\s*\\{((?:\\s*\\w+\\s*=\\s*\"(?:[^\\\\\"]|\\\\.)+\"\\s*(?:,\\s*)?)*)\\s*\\})";
        private const string Value = "?\\s+(-?\\d+(?:\\.\\d+)?(?:[eE]-?\\d+)?|-?Inf|NaN)\\s*(\\d+)?$";
        private const string Pattern3 = $"{Name}{Labels}{Value}";


        public MetricService(IOptions<MetricsOptions> metricOptions)
        {
            _timeToLive = TimeSpan.FromMinutes(metricOptions.Value.Minutes);
        }

        public void AddListMetricsDto(MetricRequestListDto metricsDto)
        {
            foreach (var metricDto in metricsDto.Metrics)
            {
                var metric = new MetricWithLabels(metricDto);
                metric.Report(_timeToLive);
            }
        }

        public void ReportStringMetrics(string metricsList, out string? validationErrors)
        {
            validationErrors = null;
            var splitMetricsList = metricsList.Split("\\n");
            var description = "";
            foreach (var line in splitMetricsList)
            {
                if (line.StartsWith("# TYPE"))
                    continue;

                if (line.StartsWith("# HELP"))
                {
                    ParseHelpString(line);
                    continue;
                }

                var isValid = Regex.Match(line, Pattern3);
                if (!isValid.Success)
                {
                    validationErrors = line + " is not valid";
                    continue;
                }

                var metric = ParseSingleMetric(line, description);
                metric.Report(_timeToLive);
            }

            void ParseHelpString(string line)
            {
                var helpLineSplit = line.Split("# HELP")
                    .LastOrDefault()
                    ?.Split(" ");

                if (helpLineSplit is null || helpLineSplit.Length <= 1) return;
                var help = helpLineSplit.Skip(1);
                description = help.Aggregate((x, y) => x + y);
            }
        }

        private IReportableMetric ParseSingleMetric(string metric, string description = "")
        {
            var splitMetric = metric.Split('{');
            if (splitMetric.Length < 2)
            {
                var splitMetricNameValue = metric.Split(' ');
                var name = splitMetricNameValue[0];
                var value = double.Parse(splitMetricNameValue[1]);

                return new MetricWithoutLabels
                {
                    Name = name,
                    Value = value,
                    Description = description
                };
            }

            var splitMetricName = metric.Split("{");
            var splitMetricValue = splitMetricName[1].Split("}");
            var labels = ParseLabels();

            return new MetricWithLabels
            {
                Name = splitMetricName[0].Trim(),
                Description = description,
                Labels = labels,
                Value = double.Parse(splitMetricValue[1].Trim())
            };

            Dictionary<string, string> ParseLabels()
            {
                var splitMetricLabels = splitMetricValue[0].Split(",");
                var chars = new[] { '\\', '\"' };
                return splitMetricLabels
                    .Select(item => item.Split("="))
                    .ToDictionary(
                        label => label[0].Trim(chars),
                        label => label[1].Trim(chars));
            }
        }
    }
}
