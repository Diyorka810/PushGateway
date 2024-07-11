using System.Runtime.Serialization;

namespace PushGateway.Model
{
    [DataContract]
    public class MetricRequestListDto
    {
        [DataMember(Name = "metrics")]
        public List<MetricRequestDto> Metrics { get; set; }
    }
}
