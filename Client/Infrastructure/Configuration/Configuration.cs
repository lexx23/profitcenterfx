namespace Client.Infrastructure.Configuration
{
    public class ConfigurationSet
    {
        public int ReceiveDelay { get; set; }
        public int MulticastPort { get; set; }
        public string MulticastGroup { get; set; }

        public ConfigurationSet()
        {
            ReceiveDelay = 0;
            MulticastPort = 2222;
            MulticastGroup = "239.0.0.222";
        }
    }
}