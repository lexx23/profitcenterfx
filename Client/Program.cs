
using Client.Indicators;
using Client.Infrastructure.Configuration;
using System.Collections.Generic;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfig config = new XmlConfig();
            ConfigurationSet configuration = config.Load();

            var indicators = new List<IIndicator>();
            indicators.Add(new MeanDeviationIndicator());
            indicators.Add(new ModeMedianIndicator());

            using (Runner runner = new Runner(configuration, indicators))
            {
                runner.Run();
            }
        }
    }
}
