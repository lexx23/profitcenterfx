namespace Client.Infrastructure
{
    internal sealed class DatagramStatistic
    {
        /// <summary>
        /// Идентификатор последней датаграммы.
        /// </summary>
        private uint _lastDatagramId;
        public uint LastDatagramId { get => _lastDatagramId; }


        /// <summary>
        /// Потеряно датаграмм.
        /// </summary>
        private ulong _lostDatagrams;
        public ulong LostDatagrams { get => _lostDatagrams; }

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

        public DatagramStatistic()
        {
            Reset();
        }

        /// <summary>
        /// Конструктор для тестирования.
        /// </summary>
        /// <param name="totalDatagrams"></param>
        /// <param name="totalDatagramsSize"></param>
        /// <param name="lostDatagrams"></param>
        /// <param name="lastDatagramId"></param>
        internal DatagramStatistic(ulong totalDatagrams, ulong totalDatagramsSize, ulong lostDatagrams, uint lastDatagramId)
        {
            _lostDatagrams = lostDatagrams;
            _totalDatagrams = totalDatagrams;
            _totalDatagramsSize = totalDatagramsSize;
            _lastDatagramId = lastDatagramId;
        }

        /// <summary>
        /// Сброс статистики.
        /// </summary>
        public void Reset()
        {
            _lostDatagrams = 0;
            _totalDatagrams = 0;
            _totalDatagramsSize = 0;
        }

        /// <summary>
        /// Обновление статистики получения и потерь пакетов.
        /// </summary>
        /// <param name="datagramId">Идентификатор пакета.</param>
        /// <param name="size">Размер датаграммы.</param>
        public void Update(uint datagramId, long size)
        {
            // Первая датаргамма, просто регистрирую данные.
            if (_totalDatagrams == 0)
            {
                _lastDatagramId = datagramId;
                _totalDatagrams++;
                _totalDatagramsSize += (ulong)size;

                return;
            }


            long difference = (long)datagramId - (long)_lastDatagramId;


            // Случай когда идентификатор перевалил за свое максимальное значение. Или пакет пришел не в том порядке.
            if (difference < 0)
            {
                // Случай с переходом счетчика на 0. Перевалили максимальное значение.
                if ((difference * -1) >= uint.MaxValue / 2)
                {
                    _lostDatagrams = (uint.MaxValue - _lastDatagramId) + datagramId;
                    _lastDatagramId = datagramId;
                }
            }
            else
            {
                _lastDatagramId = datagramId;
                if (difference > 1)
                    _lostDatagrams += (ulong)difference;
            }

            _totalDatagrams++;
            _totalDatagramsSize += (ulong)size;
        }
    }
}
