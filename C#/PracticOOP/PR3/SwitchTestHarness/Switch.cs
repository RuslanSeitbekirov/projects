namespace SwitchDevices;

public enum CoolantSystemStatus { OK, Check, Fail }
public enum SuccessFailureResult { Success, Fail }

public class Switch
{
    /// <summary>Вспомогательный генератор для имитации.</summary>
    private Random rand = new Random();

    /// <summary>Отключение от внешних систем генерации энергии.</summary>
    /// <exception cref="PowerGeneratorCommsException">Нет связи с системой генерации.</exception>
    public SuccessFailureResult DisconnectPowerGenerator()
    {
        SuccessFailureResult r = SuccessFailureResult.Fail;
        if (rand.Next(1, 10) > 2) r = SuccessFailureResult.Success;
        if (rand.Next(1, 20) > 18)
            throw new PowerGeneratorCommsException("Network failure accessing Power Generator monitoring system");
        return r;
    }

    /// <summary>Диагностика основной системы охлаждения.</summary>
    public CoolantSystemStatus VerifyPrimaryCoolantSystem()
    {
        CoolantSystemStatus c = CoolantSystemStatus.Fail;
        int r = rand.Next(1, 10);
        if (r > 5) c = CoolantSystemStatus.OK;
        else if (r > 2) c = CoolantSystemStatus.Check;

        if (rand.Next(1, 20) > 18)
            throw new CoolantTemperatureReadException("Failed to read primary coolant system temperature");
        if (rand.Next(1, 20) > 18)
            throw new CoolantPressureReadException("Failed to read primary coolant system pressure");
        return c;
    }

    /// <summary>Диагностика резервной системы охлаждения.</summary>
    public CoolantSystemStatus VerifyBackupCoolantSystem()
    {
        CoolantSystemStatus c = CoolantSystemStatus.Fail;
        int r = rand.Next(1, 10);
        if (r > 5) c = CoolantSystemStatus.OK;
        else if (r > 2) c = CoolantSystemStatus.Check;

        if (rand.Next(1, 20) > 19)
            throw new CoolantTemperatureReadException("Failed to read backup coolant system temperature");
        if (rand.Next(1, 20) > 19)
            throw new CoolantPressureReadException("Failed to read backup coolant system pressure");
        return c;
    }

    /// <summary>Чтение температуры активной зоны реактора.</summary>
    public double GetCoreTemperature()
    {
        if (rand.Next(1, 20) > 18)
            throw new CoreTemperatureReadException("Failed to read core reactor system temperature");
        return rand.NextDouble() * 1000;
    }

    /// <summary>Введение управляющих стержней для остановки реактора.</summary>
    public SuccessFailureResult InsertRodCluster()
    {
        SuccessFailureResult r = SuccessFailureResult.Fail;
        if (rand.Next(1, 100) > 5) r = SuccessFailureResult.Success;
        if (rand.Next(1, 10) > 8)
            throw new RodClusterReleaseException("Sensor failure, cannot verify rod release");
        return r;
    }

    /// <summary>Чтение уровня радиации в активной зоне.</summary>
    public double GetRadiationLevel()
    {
        if (rand.Next(1, 20) > 18)
            throw new CoreRadiationLevelReadException("Failed to read core reactor system radiation levels");
        return rand.NextDouble() * 500;
    }

    /// <summary>Оповещение «остановка завершена» по системе громкой связи.</summary>
    public void SignalShutdownComplete()
    {
        if (rand.Next(1, 20) > 18)
            throw new SignallingException("Network failure connecting to broadcast systems");
    }
}

// ----- Собственные исключения -----
public class PowerGeneratorCommsException : Exception
{
    public PowerGeneratorCommsException(string message) : base(message) { }
}

public class CoolantSystemException : Exception
{
    public CoolantSystemException(string message) : base(message) { }
}

public class CoolantTemperatureReadException : CoolantSystemException
{
    public CoolantTemperatureReadException(string message) : base(message) { }
}

public class CoolantPressureReadException : CoolantSystemException
{
    public CoolantPressureReadException(string message) : base(message) { }
}

public class CoreTemperatureReadException : Exception
{
    public CoreTemperatureReadException(string message) : base(message) { }
}
 
public class CoreRadiationLevelReadException : Exception
{
    public CoreRadiationLevelReadException(string message) : base(message) { }
}

public class RodClusterReleaseException : Exception
{
    public RodClusterReleaseException(string message) : base(message) { }
}

public class SignallingException : Exception
{
    public SignallingException(string message) : base(message) { }
}