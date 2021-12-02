using Client.Infrastructure.Configuration;
using NUnit.Framework;
using System.IO;

namespace ClientTest
{
    class XmlConfigTest
    {
        [Test]
        public void LoadNotExistConfig()
        {
            if(File.Exists("config.xml"))
                File.Delete("config.xml");

            XmlConfig config = new XmlConfig();
            var configuration = config.Load();

            Assert.NotNull(configuration);
        }

        [Test]
        public void SaveConfig()
        {
            if (File.Exists("config.xml"))
                File.Delete("config.xml");

            XmlConfig config = new XmlConfig();
            config.Save(new ConfigurationSet());

            Assert.IsTrue(File.Exists("config.xml"));
            File.Delete("config.xml");
        }
    }
}
