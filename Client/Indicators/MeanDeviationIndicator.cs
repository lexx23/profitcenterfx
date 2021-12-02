using System;
using System.Collections.Generic;

namespace Client.Indicators
{
    internal sealed class MeanDeviationIndicator : IIndicator
    {
        private ulong _counter;
        private double _oldM, _newM, _oldS, _newS;

        private readonly IDictionary<string, double> _results;
        public IReadOnlyDictionary<string, double> Results => CreateResults();

        public ulong Counter => _counter;

        public MeanDeviationIndicator()
        {
            _counter = 0;

            _results = new Dictionary<string, double>
            {
                {"Среднее", Double.NaN},
                {"Стандартное отклонение", Double.NaN}
            };
        }

        /// <summary>
        /// Таблица результатов.
        /// </summary>
        /// <returns></returns>
        private IReadOnlyDictionary<string, double> CreateResults()
        {

            if (_counter > 0)
                _results["Среднее"] = _newM;

            if (_counter > 1)
                _results["Стандартное отклонение"] = Math.Sqrt(_newS / (_counter - 1));

            return (IReadOnlyDictionary<string, double>)_results;
        }

        /// <summary>
        /// Расчет индикатора
        /// </summary>
        /// <param name="x"></param>
        public void Calculate(int x)
        {
            _counter++;

            if (_counter == 1)
            {
                _oldM = _newM = x;
                _oldS = 0.0;
            }
            else
            {
                _newM = _oldM + (x - _oldM) / _counter;
                _newS = _oldS + (x - _oldM) * (x - _newM);

                _oldM = _newM;
                _oldS = _newS;
            }
        }

        /// <summary>
        /// Сброс значений.
        /// </summary>
        public void Reset()
        {
            _counter = 0;
            _results["Среднее"] = Double.NaN;
            _results["Стандартное отклонение"] = Double.NaN;
        }
    }
}
