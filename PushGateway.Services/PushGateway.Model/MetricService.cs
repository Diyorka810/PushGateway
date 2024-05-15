using Microsoft.Extensions.Configuration;
using Prometheus;
using PushGateway.Model;
using System.Configuration;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace PushGateway.Model
{
    public class MetricService : IMetricService
    {
        private readonly TimeSpan _timeToLive;
        private const string Word = @"(\w+)";
        private const string Number = @"\d";
        private const string IntOrDouble = @$"({Number}+(?:\.{Number}+)?)";
        private const string EndOfLine = "$";
        private const string Space = @"\s";
        private const string Labels = $"(\"{Word}\"=\"{Word}\",{Space})*(?:(\"{Word}\"=\"{Word}\")+)?";
        private const string Pattern = $"{Word}{{{Labels}}}{Space}{IntOrDouble}{EndOfLine}";

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
            var splitMetricsList = metricsList.Split("\n");
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

            var res = Regex.Match(metric, Pattern);
            Console.WriteLine(res.Success);

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