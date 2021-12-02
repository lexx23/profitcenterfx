using System;
using System.Net.Sockets;
using Client.Infrastructure;
using System.Collections.Generic;
using Client.Indicators;
using System.Collections.Concurrent;
using System.Net;
using Client.Infrastructure.Configuration;

namespace Client
{
    sealed class Runner : IDisposable
    {
        private readonly ConfigurationSet _configuration;
        private readonly MulticastClient _multicastClient;
        private readonly IndicatorsThread _indicatorsThread;
        private readonly ConcurrentQueue<int> _dataQ;

        private readonly IEnumerable<IIndicator> _indicators;

        public Runner(ConfigurationSet configuration,IEnumerable<IIndicator> indicators)
        {
            _configuration = configuration ?? throw new ArgumentNullException("Не задан аргумент ConfigurationSet configuration");
            _indicators = indicators ?? throw new ArgumentNullException("Не задан аргумент IEnumerable<IIndicator> indicators");

            _dataQ = new ConcurrentQueue<int>();

            _multicastClient = new MulticastClient(new IPEndPoint(IPAddress.Any, _configuration.MulticastPort), sizeof(int) * 2);
            _multicastClient.OnError += MulticastOnError;
            _multicastClient.OnReceive += MulticastOnReceive;

            _indicatorsThread = new IndicatorsThread(_indicators,_dataQ);
        }

        /// <summary>
        /// Запуск клиента.
        /// </summary>
        public void Run()
        {
            _indicatorsThread.Start();

            _multicastClient.JointToMulticast(_configuration.MulticastGroup);
            _multicastClient.StartReceive(_configuration.ReceiveDelay);

            Console.WriteLine("! - выход");
            Console.WriteLine("i - статистика датаграмм");
            Console.WriteLine("r - рестарт подключения");
            Console.WriteLine("m - расчеты индикаторов");

            while (true)
            {
                string line = Console.ReadLine();

                if (line == "i")
                {
                    Console.WriteLine($"Потерянно датаграмм:{_multicastClient.Statistic.LostDatagrams}");
                    Console.WriteLine($"Всего принято датаграмм:{_multicastClient.Statistic.TotalDatagrams}");
                    Console.WriteLine($"Размер принятых датаграмм (байт):{_multicastClient.Statistic.TotalDatagramsSize}");
                }

                if (line == "m")
                {
                    foreach (var indicator in _indicators)
                    {
                        foreach (var indicatorResult in indicator.Results)
                            Console.WriteLine($"Индикатор {indicatorResult.Key}:{indicatorResult.Value}");
                    }
                }

                if (line == "!")
                    return;
            }
        }

        private void MulticastOnError(SocketError error)
        {
            Console.WriteLine($"Socket error: {error.ToString()}");
        }

        private void MulticastOnReceive(byte[] buffer, long size)
        {
            var value = BitConverter.ToInt32(buffer, 4);
            _dataQ.Enqueue(value);
        }

        public void Dispose()
        {
            _multicastClient.OnReceive -= MulticastOnReceive;
            _multicastClient.OnError -= MulticastOnError;
            _multicastClient.Dispose();
            _indicatorsThread.Stop();
            _indicatorsThread.Dispose();
        }
    }
}
