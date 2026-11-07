namespace StressTest;

/// <summary>Материалы балок.</summary>
public enum Material { StainlessSteel = 2, Aluminum, ReinforcedConcrete, Composite = 2, Titanium }

/// <summary>Сечения балок.</summary>
public enum CrossSection { IBeam, Box, ZShaped, CShaped }

/// <summary>Результаты испытаний.</summary>
public enum TestResult { Pass, Fail }

/// <summary>Результат одного испытания на прочность.</summary>
public struct TestCaseResult
{ 
    /// <summary>Итог испытания.</summary>
    public TestResult Result;

    /// <summary>Причина отказа (пустая строка, если испытание пройдено).</summary>
    public string ReasonForFailure;

    public TestCaseResult(TestResult result, string reasonForFailure)
    {
        Result = result;
        ReasonForFailure = reasonForFailure;
    }
}