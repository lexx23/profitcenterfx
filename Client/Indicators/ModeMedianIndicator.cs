using System;
using System.Collections.Generic;


namespace Client.Indicators
{
    internal sealed class ModeMedianIndicator : IIndicator
    {
        private readonly SortedList<int, ulong> _freqDictionary;
        private ulong _numsCount;
        private int _preColumn;
        private ulong _preCount;

        private int _maxValue;
        private ulong _maxCount;

        private readonly IDictionary<string, double> _results;
        public IReadOnlyDictionary<string, double> Results => CreateResults();

        public ModeMedianIndicator()
        {
            _freqDictionary = new SortedList<int, ulong>();

            _results = new Dictionary<string, double>
            {
                {"Мода", Double.NaN},
                {"Медиана", Double.NaN}
            };

            Reset();
        }

        /// <summary>
        /// Расчет индикатора
        /// </summary>
        /// <param name="data"></param>
        public void Calculate(int data)
        {
            var preColNum = (_freqDictionary.Count > 0) ? _freqDictionary.Keys[_preColumn] : data;
            if (data < preColNum) _preCount++;

            _numsCount++;

            if (_freqDictionary.TryGetValue(data, out ulong count))
            {
                _freqDictionary[data] = ++count;
            }
            else
            {
                if (data < preColNum)
                    _preColumn++;

                _freqDictionary[data] = 1;
                count = 1;
            }

            if (count > _maxCount)
            {
                _maxValue = data;
                _maxCount = count;
            }


            ulong medianCount = _numsCount / 2 + (_numsCount % 2);

            if (_preCount + _freqDictionary.Values[_preColumn] < medianCount)
            {
                _preCount += _freqDictionary.Values[_preColumn++];
            }
            else if (medianCount <= _preCount)
            {
                _preCount -= _freqDictionary.Values[--_preColumn];
            }
        }

        /// <summary>
        /// Таблица результатов.
        /// </summary>
        /// <returns></returns>
        private IReadOnlyDictionary<string, double> CreateResults()
        {
            if (_freqDictionary.Count == 0)
            {
                _results["Мода"] = Double.NaN;
                _results["Медиана"] = Double.NaN;
                return (IReadOnlyDictionary<string, double>)_results;
            }

            ulong medianCount = _numsCount / 2 + (_numsCount % 2);
            double median = 0.0;

            var num1 = _freqDictionary.Keys[_preColumn];
            var count1 = _freqDictionary.Values[_preColumn];

            if ((_preCount + count1 == medianCount) && (_numsCount % 2 == 0))
            {
                var num2 = _freqDictionary.Keys[_preColumn + 1];
                median = ((double)num1 + num2) / 2.0;
            }
            else
                median = num1;

            _results["Медиана"] = median;

            if (_maxCount != 1)
                _results["Мода"] = _maxValue;
            else
                _results["Мода"] = Double.NaN;

            return (IReadOnlyDictionary<string, double>)_results;
        }


        /// <summary>
        /// Сброс значений.
        /// </summary>
        public void Reset()
        {
            _numsCount = 0L;
            _preColumn = 0;
            _preCount = 0L;

            _maxCount = 0L;
            _maxValue = 0;

            _freqDictionary.Clear();

            _results["Мода"] = Double.NaN;
            _results["Медиана"] = Double.NaN;
        }
    }
}
