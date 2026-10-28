using System.Windows;
using SwitchDevices;

namespace SwitchTestHarness;

public partial class MainWindow : Window
{
    private readonly SwitchDevices.Switch sd = new SwitchDevices.Switch();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button1_Click(object sender, RoutedEventArgs e)
    {
        textBlock1.Text = "Shutdown started\n";

        // Step 1 - disconnect from the Power Generator
        try
        {
            SuccessFailureResult r = sd.DisconnectPowerGenerator();
            textBlock1.Text += $"Step 1 - Disconnect from Power Generator: {r}\n";
        }
        catch (PowerGeneratorCommsException ex)
        {
            textBlock1.Text += "*** Exception in step 1: " + ex.Message + "\n";
        }

        // Step 2 - Verify the status of the Primary Coolant System
        try
        {
            CoolantSystemStatus status = sd.VerifyPrimaryCoolantSystem();
            textBlock1.Text += $"Step 2 - Primary Coolant System: {status}\n";
        }
        catch (CoolantPressureReadException ex)
        {
            textBlock1.Text += "*** Exception in step 2: " + ex.Message + "\n";
        }
        catch (CoolantTemperatureReadException ex)
        {
            textBlock1.Text += "*** Exception in step 2: " + ex.Message + "\n";
        }

        // Step 3 - Verify the status of the Backup Coolant System
        try
        {
            CoolantSystemStatus status = sd.VerifyBackupCoolantSystem();
            textBlock1.Text += $"Step 3 - Backup Coolant System: {status}\n";
        }
        catch (CoolantPressureReadException ex)
        {
            textBlock1.Text += "*** Exception in step 3: " + ex.Message + "\n";
        }
        catch (CoolantTemperatureReadException ex)
        {
            textBlock1.Text += "*** Exception in step 3: " + ex.Message + "\n";
        }

        // Step 4 - Record the core temperature prior to shutting down the reactor
        try
        {
            double temp = sd.GetCoreTemperature();
            textBlock1.Text += $"Step 4 - Core temperature before shutdown: {temp:F1}\n";
        }
        catch (CoreTemperatureReadException ex)
        {
            textBlock1.Text += "*** Exception in step 4: " + ex.Message + "\n";
        }

        // Step 5 - Insert the control rods into the reactor
        try
        {
            SuccessFailureResult r = sd.InsertRodCluster();
            textBlock1.Text += $"Step 5 - Insert control rods: {r}\n";
        }
        catch (RodClusterReleaseException ex)
        {
            textBlock1.Text += "*** Exception in step 5: " + ex.Message + "\n";
        }

        // Step 6 - Record the core temperature after shutting down the reactor
        try
        {
            double temp = sd.GetCoreTemperature();
            textBlock1.Text += $"Step 6 - Core temperature after shutdown: {temp:F1}\n";
        } 
        catch (CoreTemperatureReadException ex)
        {
            textBlock1.Text += "*** Exception in step 6: " + ex.Message + "\n";
        }

        // Step 7 - Record the core radiation levels after shutting down the reactor
        try
        {
            double radiation = sd.GetRadiationLevel();
            textBlock1.Text += $"Step 7 - Core radiation level: {radiation:F1}\n";
        }
        catch (CoreRadiationLevelReadException ex)
        {
            textBlock1.Text += "*** Exception in step 7: " + ex.Message + "\n";
        }

        // Step 8 - Broadcast "Shutdown Complete" message
        try
        {
            sd.SignalShutdownComplete();
            textBlock1.Text += "Step 8 - Shutdown Complete broadcast sent\n";
        }
        catch (SignallingException ex)
        {
            textBlock1.Text += "*** Exception in step 8: " + ex.Message + "\n";
        }

        textBlock1.Text += "Shutdown sequence finished (all 8 steps attempted)\n";
    }
}