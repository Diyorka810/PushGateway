using System.Runtime.Serialization;

namespace PushGateway.Model
{
    [DataContract]
    public class MetricRequestDto
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "labels")]
        public Dictionary<string, string> Labels { get; set; }

        [DataMember(Name = "value")]
        public double Value { get; set; }

        [DataMember(Name = "description")]
        public string? Description { get; set; }
    }
}
