using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Client.Indicators
{
    /// <summary>
    /// Калькулятор индикаторов.
    /// </summary>
    internal sealed class IndicatorsThread: IDisposable
    {
        private readonly IEnumerable<IIndicator> _indicators;
        private readonly ConcurrentQueue<int> _data;
        private CancellationTokenSource _ctSource;
        private MeanDeviationIndicator _simple;
        private Thread _thread;

        public IndicatorsThread(IEnumerable<IIndicator> indicators, ConcurrentQueue<int> data)
        {
            _indicators = indicators ?? throw new ArgumentNullException("Не задан аргумент IEnumerable<IIndicator> indicators");
            _data = data ?? throw new ArgumentNullException("Не задан аргумент  ConcurrentQueue<int> data");

            _simple = new MeanDeviationIndicator();
        }

        /// <summary>
        /// Запуск просчета индикаторов в отдельном потоке.
        /// </summary>
        public void Start()
        {
            _thread = new Thread(new ThreadStart(WorkThread));
            _thread.Start();
        }

        private void WorkThread()
        {
            _ctSource = new CancellationTokenSource();

            ResetIndicators();

            while (!_ctSource.IsCancellationRequested)
            {
                while (_data.IsEmpty && !_ctSource.IsCancellationRequested)
                    Thread.Yield();

                if (_data.TryDequeue(out int value))
                {
                    foreach (var indicator in _indicators)
                    {
                        indicator.Calculate(value);
                    }
                }
            }
        }

        /// <summary>
        /// Сброс значений индикаторов.
        /// </summary>
        private void ResetIndicators()
        {
            foreach (var indicator in _indicators)
            {
                indicator.Reset();
            }
        }

        /// <summary>
        /// Остановить обработку данных.
        /// </summary>
        public void Stop()
        {
            _ctSource.Cancel();
            _thread.Join();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
