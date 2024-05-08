using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PushGateway.Model
{
    [DataContract]
    public class MetricRequestListDto
    {
        [DataMember(Name = "metrics")]
        public List<MetricRequestDto> Metrics { get; set; }
    }
}
