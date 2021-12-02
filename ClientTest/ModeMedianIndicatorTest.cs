using Client.Indicators;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ClientTest
{
    class ModeMedianIndicatorTest
    {
        [Test]
        [Description("Проверка расчета индикатора моды и медианы.")]
        public void ModeMedianTest()
        {
            var data = new List<int>()
            {
                2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5
            };

            ModeMedianIndicator indicator = new ModeMedianIndicator();

            Assert.AreEqual(Double.NaN, indicator.Results["Мода"]);
            Assert.AreEqual(Double.NaN, indicator.Results["Медиана"]);

            foreach (var i in data)
            {
                indicator.Calculate(i);
            }

            Assert.AreEqual(3, indicator.Results["Мода"]);
            Assert.AreEqual(3, indicator.Results["Медиана"]);
        }

        [Test]
        [Description("Проверка расчета индикатора медианы для четного ряда.")]
        public void EvenRowCalculateMediana()
        {
            var data = new List<int>()
            {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8
            };

            ModeMedianIndicator indicator = new ModeMedianIndicator();

            Assert.AreEqual(Double.NaN, indicator.Results["Мода"]);
            Assert.AreEqual(Double.NaN, indicator.Results["Медиана"]);

            foreach (var i in data)
            {
                indicator.Calculate(i);
            }

            Assert.AreEqual(Double.NaN, indicator.Results["Мода"]);
            Assert.AreEqual(4.5, indicator.Results["Медиана"]);
        }
    }
}
