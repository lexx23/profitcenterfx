using Client.Indicators;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ClientTest
{
    class MeanDeviationIndicatorTest
    {
        [Test]
        [Description("Проверка расчета индикатора стандартное отклонение.")]
        public void DeviationTest()
        {
            var data = new List<int>()
            {
                10,
                20,
                30,
                40,
                50,
                60,
                70
            };

            MeanDeviationIndicator indicator = new MeanDeviationIndicator();

            Assert.AreEqual(Double.NaN, indicator.Results["Стандартное отклонение"]);
            Assert.AreEqual(Double.NaN, indicator.Results["Среднее"]);

            foreach (var i in data)
            {
                indicator.Calculate(i);
            }

            Assert.AreEqual(40, indicator.Results["Среднее"]);
            Assert.AreEqual(21.60, indicator.Results["Стандартное отклонение"], 0.01);
        }

        [Test]
        [Description("Проверка расчета индикатора среднее.")]
        public void MeanTest()
        {
            var data = new List<int>()
            {
                1,
                2,
                3,
                4,
                5
            };

            MeanDeviationIndicator indicator = new MeanDeviationIndicator();

            Assert.AreEqual(Double.NaN, indicator.Results["Среднее"]);

            foreach (var i in data)
            {
                indicator.Calculate(i);
            }

            Assert.AreEqual(3, indicator.Results["Среднее"]);
        }
    }
}
