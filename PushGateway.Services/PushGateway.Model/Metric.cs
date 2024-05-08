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
        public Dictionary<string, string> Labels { get; set;}
        public string? Description { get; set; }


        public bool IsValid()
        {

            return true;
        }
    }
}
