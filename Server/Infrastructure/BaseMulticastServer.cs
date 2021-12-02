using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Infrastructure
{
    internal abstract class BaseMulticastServer : IDisposable
    {
        private readonly IPEndPoint _endpoint;
        private readonly UdpClient _udp;
        private readonly int _bufferCapacity;
        private readonly byte[] _buffer;
        private Thread _thread;

        private uint _datagramId;
       
        /// <summary>
        /// Количество принятых датаграмм.
        /// </summary>
        private ulong _totalDatagrams;
        public ulong TotalDatagrams { get => _totalDatagrams; }

        /// <summary>
        /// Размер всех принятых датаграмм.
        /// </summary>
        private ulong _totalDatagramsSize;
        public ulong TotalDatagramsSize { get => _totalDatagramsSize; }

        private bool _isStarted;
        public bool IsStarted => _isStarted;

        public BaseMulticastServer(IPEndPoint endpoint, int bufferCapacity)
        {
            _bufferCapacity = bufferCapacity;
            _buffer = new byte[bufferCapacity];

            _endpoint = endpoint;
            _udp = new UdpClient();
            _udp.JoinMulticastGroup(_endpoint.Address);
        }

        public BaseMulticastServer(string address, int port, int bufferCapacity) : this(new IPEndPoint(IPAddress.Parse(address), port), bufferCapacity) { }

        /// <summary>
        /// Запустить сервер.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (_isStarted)
                return true;

            _isStarted = true;

            _thread = new Thread(new ThreadStart(StartThread));
            _thread.Start();

            return true;
        }

        private void StartThread()
        {
            while (_isStarted)
            {
                int value = GetData();

                var datagramIdArray = BitConverter.GetBytes(_datagramId);
                datagramIdArray.CopyTo(_buffer, 0);

                var valueArray = BitConverter.GetBytes(value);
                valueArray.CopyTo(_buffer, 4);

                _udp.Send(_buffer, _bufferCapacity, _endpoint);

                if (_datagramId == uint.MaxValue)
                    _datagramId = 0;
                else
                    _datagramId++;

                _totalDatagrams++;
                _totalDatagramsSize += (ulong)_bufferCapacity;
            }
        }

        /// <summary>
        /// Остановить сервер
        /// </summary>
        public void Stop()
        {
            if (!_isStarted)
                return;

            _isStarted = false;
            _thread.Join();
        }

        /// <summary>
        /// Отправка сообщений.
        /// </summary>
        public abstract int GetData();


        public void Dispose()
        {
            Stop();
            _udp.Dispose();
        }
    }

}
