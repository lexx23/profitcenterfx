using System;
using System.Net;
using Server.Generator;
using Server.Infrastructure;
using Server.Infrastructure.Configuration;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfig config = new XmlConfig();
            ConfigurationSet configuration = config.Load();

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(configuration.MulticastGroup), configuration.MulticastPort);
            IValueGenerator generator = new RandomGenerator(configuration.MinValue, configuration.MaxValue);

            using (MulticastServer server = new MulticastServer(generator, endPoint, sizeof(int) * 2))
            {
                server.Start();

                Console.WriteLine("! - выход");
                Console.WriteLine("i - статистика датаграмм");

                while (true)
                {
                    string line = Console.ReadLine();

                    if (line == "i")
                    {
                        Console.WriteLine($"Всего отправленно датаграмм:{server.TotalDatagrams}");
                        Console.WriteLine($"Размер отправленных датаграмм (байт):{server.TotalDatagramsSize}");
                    }

                    if (line == "!")
                        return;
                }
            }
        }
    }
}
