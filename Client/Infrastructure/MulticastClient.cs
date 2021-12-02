using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client.Infrastructure
{
    internal sealed class MulticastClient : IDisposable
    {
        private readonly IPEndPoint _endpoint;
        private Socket _socket;
        private EndPoint _receiveEndpoint;
        private SocketAsyncEventArgs _receiveEventArg;

        private bool _receiving;
        private readonly int _bufferCapacity;
        private readonly byte[] _receiveBuffer;

        private int _delay;

        /// <summary>
        /// Статистика датаграмм (принято, пропущено,размер).
        /// </summary>
        private readonly DatagramStatistic _datagramStatistic;
        public DatagramStatistic Statistic { get => _datagramStatistic; }

        /// <summary>
        /// Состояние подключения.
        /// </summary>
        private bool _isConnected;
        public bool IsConnected { get => _isConnected; }

        /// <summary>
        /// Событие возникает при ошибке получения данных или при подключении.
        /// </summary>
        public event OnErrorEventHandler OnError;
        /// <summary>
        /// Событие возникает при получении данных.
        /// </summary>
        public event OnReceiveEventHandler OnReceive;

        public delegate void OnErrorEventHandler(SocketError error);
        public delegate void OnReceiveEventHandler(byte[] buffer, long size);



        public MulticastClient(IPEndPoint endpoint,int bufferCapacity)
        {
            _endpoint = endpoint;
            _bufferCapacity = bufferCapacity;
            _receiveBuffer = new byte[_bufferCapacity];

            _datagramStatistic = new DatagramStatistic();
        }

        public MulticastClient(string address, int port,int bufferCapacity) : this(new IPEndPoint(IPAddress.Parse(address), port),bufferCapacity) { }


        /// <summary>
        /// Подключение к мультикаст группе.
        /// </summary>
        public bool JointToMulticast(string multicastGroup)
        {
            if (_isConnected)
                return true;

            try
            {
                _datagramStatistic.Reset();

                // Инициализация сокета.
                _socket = new Socket(_endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _socket.Bind(_endpoint);

                if (_endpoint.AddressFamily == AddressFamily.InterNetworkV6)
                    _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(IPAddress.Parse(multicastGroup)));
                else
                    _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(multicastGroup)));

                _receiveEndpoint = new IPEndPoint((_endpoint.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);

                // Установка обработчика ответов и размера буфера
                _receiveEventArg = new SocketAsyncEventArgs();
                _receiveEventArg.Completed += OnAsyncCompleted;
                _receiveEventArg.RemoteEndPoint = _receiveEndpoint;
                _receiveEventArg.SetBuffer(_receiveBuffer, 0, (int)_bufferCapacity);

            }
            catch (SocketException e)
            {
                OnError?.Invoke(e.SocketErrorCode);
                return false;
            }

            _isConnected = true;
            return true;
        }

        /// <summary>
        /// Отключение клиента.
        /// </summary>
        public void Disconnect()
        {
            _receiveEventArg.Completed -= OnAsyncCompleted;

            try
            {
                _socket.Close();
                _socket.Dispose();
            }
            catch (ObjectDisposedException) { }

            _isConnected = false;
            _receiving = false;
        }

        /// <summary>
        /// Начать получение данных.
        /// </summary>
        public void StartReceive(int delay)
        {
            _delay = delay;

            if (_receiving)
                return;

            if (!_isConnected)
                return;

            try
            {
                _receiving = true;

                // Цикл необходим для случая когда происходит не асинхронное получение данных. 
                while (_isConnected)
                {
                    if (!_socket.ReceiveFromAsync(_receiveEventArg))
                        ProcessReceiveFrom(_receiveEventArg, false);
                    else
                        return;
                }

            }
            catch (ObjectDisposedException) { }
        }


        /// <summary>
        /// Событие асинхронного получения пакета.
        /// </summary>
        private void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceiveFrom(e,true);
        }

        /// <summary>
        /// Обработка полученного пакета.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="async">Признак синхронного или асинхронного вызова.</param>
        private void ProcessReceiveFrom(SocketAsyncEventArgs e,bool async)
        {
            _receiving = false;

            if (!_isConnected)
                return;

            // При ошибке отключаемся.
            if (e.SocketError != SocketError.Success)
            {
                OnError?.Invoke(e.SocketError);
                Disconnect();
                return;
            }

            long size = e.BytesTransferred;

            // Обработка полученных данных.
            if (size > 0)
            {
                // Идентификатор пакета.
                uint datagramId = BitConverter.ToUInt32(_receiveBuffer);
                // Обновление статистики.
                _datagramStatistic.Update(datagramId,size);


                // Повторный запуск получения сообщений, вызывается только в том случае если эта функция отрабатывает в асинхронном режиме. Если вызывать в синхронном будет stackoverflow.
                if (async)
                {
                    if(_delay != 0)
                        Thread.Sleep(_delay);
                    StartReceive(_delay);
                }

                // Вызов клиентского события.
                OnReceive?.Invoke(_receiveBuffer, size);
            }
        }


        public void Dispose()
        {
            Disconnect();
        }
    }
}
