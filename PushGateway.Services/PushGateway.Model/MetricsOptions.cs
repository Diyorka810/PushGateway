namespace PushGateway.Model
{
    public class MetricsOptions
    {
        public const string Position = "MetricsTimeToLive";
        public TimeSpan Minutes { get; set; }
    }
}
