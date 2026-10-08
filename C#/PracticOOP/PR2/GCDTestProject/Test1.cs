using Microsoft.VisualStudio.TestTools.UnitTesting;

// Псевдонимы нужны, потому что в обоих проектах класс называется MainWindow.
// ВНИМАНИЕ: в имени второго пространства имён буква "О" — кириллическая
// (как в вашем проекте). Лучше скопируйте имя из вашего MainWindow.xaml.cs.
using Gcd2 = GreatestCommonDivisor.MainWindow;               // Задание 1: два числа
using GcdOverloaded = GreatestCommonDivisorОverloading.MainWindow; // Задание 2: перегрузки

namespace GCDTestProject
{
    // ===================================================================
    // Практическая работа №2, Задание 1: метод Евклида для двух чисел
    // ===================================================================
    [TestClass]
    public class GCDEuclidTwoNumbersTests
    {
        // Тест из методички: a = 2806, b = 345, expected = 23
        [TestMethod]
        public void FindGCDEuclidTest()
        {
            int a = 2806;
            int b = 345;
            int expected = 23;

            int actual = Gcd2.CalculateGCD(a, b);

            Assert.AreEqual(expected, actual);
        }

        // Таблица проверочных значений из методички (шаг 7 задания 1)
        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 10, 10)]
        [DataRow(25, 10, 5)]
        [DataRow(25, 100, 25)]
        [DataRow(26, 100, 2)]
        [DataRow(27, 100, 1)]
        public void FindGCDEuclidTable(int a, int b, int expected)
        {
            Assert.AreEqual(expected, Gcd2.CalculateGCD(a, b));
        }
    }

    // ===================================================================
    // Практическая работа №2, Задание 2: перегрузки для 3, 4 и 5 чисел
    // (в методичке метод называется FindGCDEuclid, в вашем коде — CalculateGCD)
    // ===================================================================
    [TestClass]
    public class GCDEuclidOverloadTests
    {
        // Три числа: 7396, 1978, 1204 -> 86
        [TestMethod]
        public void FindGCDEuclidTest1()
        {
            int a = 7396, b = 1978, c = 1204;
            int expected = 86;

            int actual = GcdOverloaded.CalculateGCD(a, b, c);

            Assert.AreEqual(expected, actual);
        }

        // Четыре числа: 7396, 1978, 1204, 430 -> 86
        [TestMethod]
        public void FindGCDEuclidTest2()
        {
            int a = 7396, b = 1978, c = 1204, d = 430;
            int expected = 86;

            int actual = GcdOverloaded.CalculateGCD(a, b, c, d);

            Assert.AreEqual(expected, actual);
        }

        // Пять чисел: 7396, 1978, 1204, 430, 258 -> 86
        [TestMethod]
        public void FindGCDEuclidTest3()
        {
            int a = 7396, b = 1978, c = 1204, d = 430, e = 258;
            int expected = 86;

            int actual = GcdOverloaded.CalculateGCD(a, b, c, d, e);

            Assert.AreEqual(expected, actual);
        }

        // Таблица проверочных значений из методички (шаг 7 задания 2), пять чисел
        [TestMethod]
        [DataRow(2806, 345, 0, 0, 0, 23)]
        [DataRow(0, 0, 0, 0, 0, 0)]
        [DataRow(0, 0, 0, 0, 1, 1)]
        [DataRow(12, 24, 36, 48, 60, 12)]
        [DataRow(13, 24, 36, 48, 60, 1)]
        [DataRow(14, 24, 36, 48, 60, 2)]
        [DataRow(15, 24, 36, 48, 60, 3)]
        [DataRow(16, 24, 36, 48, 60, 4)]
        [DataRow(0, 24, 36, 48, 60, 12)]
        public void FindGCDEuclidFiveNumbersTable(int a, int b, int c, int d, int e, int expected)
        {
            Assert.AreEqual(expected, GcdOverloaded.CalculateGCD(a, b, c, d, e));
        }
    }
}