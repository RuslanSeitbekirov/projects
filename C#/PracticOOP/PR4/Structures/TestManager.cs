namespace StressTest;

/// <summary>Имитирует проведение испытаний на прочность.</summary>
public static class TestManager
{
    private static readonly string[] FailureReasons =
    {
        "Stress exceeded tolerance limit",
        "Girder buckled under load",
        "Crack detected in weld seam",
        "Excessive deflection at mid-span",
        "Fatigue failure after cyclic loading",
        "Material defect found during inspection"
    };

    /// <summary>Возвращает результат одного испытания (случайно).</summary>
    public static TestCaseResult GenerateResult()
    {
        // Примерно 70% испытаний проходят успешно
        if (Random.Shared.Next(100) < 70)
            return new TestCaseResult(TestResult.Pass, string.Empty);

        string reason = FailureReasons[Random.Shared.Next(FailureReasons.Length)];
        return new TestCaseResult(TestResult.Fail, reason);
    }
}