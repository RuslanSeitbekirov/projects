using GreatestCommonDivisorStein;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GCDTestProject
{
    [TestClass]
    public class GCDAlgorithmsTest
    {
        // Тесты из методички (шаг 4, пп. 11 и 17)
        [TestMethod]
        public void FindGCDEuclidTest()
        {
            int u = 298467352, v = 569484, expected = 4;
            int actual = GCDAlgorithms.FindGCDEuclid(u, v, out long time);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindGCDSteinTest()
        {
            int u = 298467352, v = 569484, expected = 4;
            int actual = GCDAlgorithms.FindGCDStein(u, v, out long time);
            Assert.AreEqual(expected, actual);
        }

        // Оба алгоритма должны давать одинаковый результат
        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 10, 10)]
        [DataRow(25, 10, 5)]
        [DataRow(25, 100, 25)]
        [DataRow(26, 100, 2)]
        [DataRow(27, 100, 1)]
        [DataRow(2806, 345, 23)]
        public void BothAlgorithmsAgree(int a, int b, int expected)
        {
            Assert.AreEqual(expected, GCDAlgorithms.FindGCDEuclid(a, b, out _));
            Assert.AreEqual(expected, GCDAlgorithms.FindGCDStein(a, b, out _));
        }

        // Тренировочное задание: выражение из цифры 2 вычисляется в нужное число
        [TestMethod]
        public void PrimeExpression_EvaluatesToOriginalNumber()
        {
            for (int n = 1; n <= 5000; n++)
            {
                string expr = PrimeShiftExpression.Build(n);
                Assert.IsTrue(PrimeShiftExpression.ContainsOnlyDigitTwo(expr), "n = " + n);
                Assert.AreEqual(n, PrimeShiftExpression.Evaluate(expr), "n = " + n);
            }
        }

        [TestMethod]
        [DataRow(100, 97)]
        [DataRow(14, 13)]
        [DataRow(3, 2)]
        [DataRow(2147483647, 2147483629)]
        public void LargestPrimeBelow(int n, int expected)
        {
            Assert.AreEqual(expected, PrimeShiftExpression.FindLargestPrimeBelow(n));
        }

        [TestMethod]
        public void LargestPrimeBelow_NoPrime()
        {
            Assert.IsNull(PrimeShiftExpression.FindLargestPrimeBelow(2));
        }
    }
}