using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Infrastructure.Configuration
{
    public class ConfigurationSet
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int MulticastPort { get; set; }
        public string MulticastGroup { get; set; }

        public ConfigurationSet()
        {
            MinValue = 0;
            MaxValue = 100;
            MulticastPort = 2222;
            MulticastGroup = "239.0.0.222";
        }
    }
}
