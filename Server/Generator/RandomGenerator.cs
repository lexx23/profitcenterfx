using System;

namespace Server.Generator
{
    /// <summary>
    /// Генератор случайных чисел.
    /// </summary>
    internal sealed class RandomGenerator : IValueGenerator
    {
        private readonly Random _random;
        private readonly int _min;
        private readonly int _max;

        public RandomGenerator(int min, int max)
        {
            _min = min;
            _max = max;
            _random = new Random(100);
        }

        /// <summary>
        /// Получить следующее значение.
        /// </summary>
        /// <returns></returns>
        public int Next()
        {
            return _random.Next(_min, _max);
        }
    }
}
