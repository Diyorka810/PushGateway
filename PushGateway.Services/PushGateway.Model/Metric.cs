using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushGateway.Model
{
    public class Metric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public Dictionary<string, string>? Labels { get; set; }
        public string? Description { get; set; }

        public Metric(MetricRequestDto metricDto)
        {
            Name = metricDto.Name;
            Value = metricDto.Value;
            Labels = metricDto.Labels;
            Description = metricDto.Description;
        }

        public Metric(string metric, string descr = "")
        {
            var splitMetric = metric.Split('{');
            if (splitMetric.Length < 2)
            {
                var splitMetricNameValue = metric.Split(' ');
                Name = splitMetricNameValue[0];
                Value = double.Parse(splitMetricNameValue[1]);
            }
            else if (splitMetric.Length > 2)
            {

            }
            else
            {
                var splitMetricName = metric.Split("{");
                Name = splitMetricName[0];
                var splitMetricValue = splitMetricName[1].Split("}");
                Value = double.Parse(splitMetricValue[1].Trim());

                var splitMetricLabels = splitMetricValue[0].Split(",");
                var dict = new Dictionary<string, string>();
                var chars = new Char[] { '\\', '\"' };
                foreach (var item in splitMetricLabels)
                {
                    var label = item.Split("=");
                    dict.Add(label[0].Trim(chars), label[1].Trim(chars));
                }
                Labels = dict;
                Description = descr;
            }
        }
    }
}
