using System.Windows;
using StressTest;

namespace Structures;

public partial class MainWindow : Window
{
    // Task 3: массив результатов испытаний
    private TestCaseResult[] results = Array.Empty<TestCaseResult>();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void RunTests_Click(object sender, RoutedEventArgs e)
    {
        reasonsList.Items.Clear();

        // Task 4: создаём массив из 10 элементов и заполняем его результатами испытаний
        results = new TestCaseResult[10];
        for (int i = 0; i < results.Length; i++)
        {
            results[i] = TestManager.GenerateResult();
        }

        // Task 5: подсчёт успехов и отказов, вывод причин отказов
        int passCount = 0;
        int failCount = 0;

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].Result == TestResult.Pass)
            {
                passCount++;
            }
            else if (results[i].Result == TestResult.Fail)
            {
                failCount++;
                reasonsList.Items.Add($"Test {i + 1}: {results[i].ReasonForFailure}");
            }
        }

        successes.Content = $"Successes: {passCount}";
        failures.Content = $"Failures: {failCount}";
    }
}