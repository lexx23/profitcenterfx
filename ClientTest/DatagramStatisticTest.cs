using Client.Infrastructure;
using NUnit.Framework;

namespace ClientTest
{
    public class DatagramStatisticTest
    {
        [Test]
        [Description("�������� ��������� ��� ���������� ����� ����������.")]
        public void OneDatagramAdd()
        {
            DatagramStatistic statistic = new DatagramStatistic();

            statistic.Update(1, 4);

            Assert.AreEqual(0, statistic.LostDatagrams);
            Assert.AreEqual(1, statistic.TotalDatagrams);
            Assert.AreEqual(4, statistic.TotalDatagramsSize);
        }

        [Test]
        [Description("�������� ��������� ��� ���������� ���� ���������.")]
        public void TwoDatagramAdd()
        {
            DatagramStatistic statistic = new DatagramStatistic();

            statistic.Update(1, 4);

            Assert.AreEqual(0, statistic.LostDatagrams);
            Assert.AreEqual(1, statistic.TotalDatagrams);
            Assert.AreEqual(4, statistic.TotalDatagramsSize);

            statistic.Update(2, 4);

            Assert.AreEqual(0, statistic.LostDatagrams);
            Assert.AreEqual(2, statistic.TotalDatagrams);
            Assert.AreEqual(8, statistic.TotalDatagramsSize);
        }

        [Test]
        [Description("�������� ��������� �� �����������.")]
        public void DatagramOverflowTest()
        {
            DatagramStatistic statistic = new DatagramStatistic(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, 0);

            statistic.Update(2, 4);

            Assert.AreEqual(1, statistic.LostDatagrams);
            Assert.AreEqual(0, statistic.TotalDatagrams);
            Assert.AreEqual(3, statistic.TotalDatagramsSize);
            Assert.AreEqual(2, statistic.LastDatagramId);
        }

        [Test]
        [Description("�������� ���������, ��� ����������� ������ ����������.")]
        public void FirstDatagram()
        {
            DatagramStatistic statistic = new DatagramStatistic();

            statistic.Update(100, 4);

            Assert.AreEqual(0, statistic.LostDatagrams);
            Assert.AreEqual(1, statistic.TotalDatagrams);
            Assert.AreEqual(4, statistic.TotalDatagramsSize);
            Assert.AreEqual(100, statistic.LastDatagramId);
        }

        [Test]
        [Description("�������� �������� ������ �������.")]
        public void SimpleDatagramLost()
        {
            DatagramStatistic statistic = new DatagramStatistic(1, 4, 0, 100);

            statistic.Update(103, 4);

            Assert.AreEqual(3, statistic.LostDatagrams);
            Assert.AreEqual(2, statistic.TotalDatagrams);
            Assert.AreEqual(8, statistic.TotalDatagramsSize);
            Assert.AreEqual(103, statistic.LastDatagramId);
        }

        [Test]
        [Description("�������� �������� ������ �������, � ������ ���� ����� �������.")]
        public void OldDatagramLostCounterTest()
        {
            DatagramStatistic statistic = new DatagramStatistic(1, 4, 0, 150);

            statistic.Update(90, 4);

            Assert.AreEqual(0, statistic.LostDatagrams);
            Assert.AreEqual(2, statistic.TotalDatagrams);
            Assert.AreEqual(8, statistic.TotalDatagramsSize);
            Assert.AreEqual(150, statistic.LastDatagramId);
        }


        [Test]
        [Description("�������� �������� ������ �������, � ������ ������� ��������� �� ������������ �����.")]
        public void LostCounterWhenDatagramIdOverMaxValueTest()
        {
            DatagramStatistic statistic = new DatagramStatistic(1, 4, 0, uint.MaxValue - 100);

            statistic.Update(150, 4);

            Assert.AreEqual(250, statistic.LostDatagrams);
            Assert.AreEqual(2, statistic.TotalDatagrams);
            Assert.AreEqual(8, statistic.TotalDatagramsSize);
            Assert.AreEqual(150, statistic.LastDatagramId);
        }

    }
}