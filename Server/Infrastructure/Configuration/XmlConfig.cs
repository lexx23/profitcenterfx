using System.IO;
using System.Xml.Serialization;

namespace Server.Infrastructure.Configuration
{
    internal sealed class XmlConfig
    {
        private XmlSerializer _formatter;

        public XmlConfig()
        {
            _formatter = new XmlSerializer(typeof(ConfigurationSet));
        }

        public ConfigurationSet Load()
        {
            try
            {
                using (FileStream fs = new FileStream("config.xml", FileMode.OpenOrCreate))
                {
                    ConfigurationSet config = (ConfigurationSet) _formatter.Deserialize(fs);
                    return config;
                }
            }
            catch
            {
                var configuration = new ConfigurationSet();
                Save(configuration);
                return configuration;
            }
        }


        public void Save(ConfigurationSet config)
        {
            using (FileStream fs = new FileStream("config.xml", FileMode.OpenOrCreate))
            {
                _formatter.Serialize(fs, config);
            }
        }
    }
}
