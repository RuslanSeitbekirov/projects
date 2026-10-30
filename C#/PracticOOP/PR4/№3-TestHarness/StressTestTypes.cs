namespace StressTest;

/// <summary>Материалы балок.</summary>
public enum Material
{
    StainlessSteel,
    Aluminum,
    ReinforcedConcrete,
    Composite,
    Titanium
}

/// <summary>Сечения балок.</summary>
public enum CrossSection
{ 
    IBeam,
    Box,
    ZShaped,
    CShaped
}

/// <summary>Результаты испытаний.</summary>
public enum TestResult
{
    Pass,
    Fail
}   