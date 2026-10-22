using System.Text;
using System.Windows;
using System.Windows.Controls;
using StressTest;

namespace TestHarness;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Task 3: списки заполняются значениями перечислений
        materials.ItemsSource = Enum.GetValues(typeof(Material));
        crosssections.ItemsSource = Enum.GetValues(typeof(CrossSection));
        testresults.ItemsSource = Enum.GetValues(typeof(TestResult));
    }

    private void selectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Пока выбраны не все три значения, выводить нечего
        if (materials.SelectedItem == null ||
            crosssections.SelectedItem == null ||
            testresults.SelectedItem == null)
        {
            testDetails.Content = "Выберите по одному значению из каждого списка";
            return;
        }

        // Task 3: SelectedItem имеет тип object, поэтому его приводим к типу перечисления
        Material selectedMaterial = (Material)materials.SelectedItem;
        CrossSection selectedCrossSection = (CrossSection)crosssections.SelectedItem;
        TestResult selectedTestResult = (TestResult)testresults.SelectedItem;

        // Task 4: строим строку описания
        var selectionStringBuilder = new StringBuilder();

        switch (selectedMaterial)
        {
            case Material.StainlessSteel:
                selectionStringBuilder.Append("Material: Stainless Steel, ");
                break;
            case Material.Aluminum:
                selectionStringBuilder.Append("Material: Aluminum, ");
                break;
            case Material.ReinforcedConcrete:
                selectionStringBuilder.Append("Material: Reinforced Concrete, ");
                break;
            case Material.Composite:
                selectionStringBuilder.Append("Material: Composite, ");
                break;
            case Material.Titanium:
                selectionStringBuilder.Append("Material: Titanium, ");
                break;
        }

        switch (selectedCrossSection)
        {
            case CrossSection.IBeam:
                selectionStringBuilder.Append("Cross-section: I-Beam, ");
                break;
            case CrossSection.Box:
                selectionStringBuilder.Append("Cross-section: Box, ");
                break;
            case CrossSection.ZShaped:
                selectionStringBuilder.Append("Cross-section: Z-Shaped, ");
                break;
            case CrossSection.CShaped:
                selectionStringBuilder.Append("Cross-section: C-Shaped, ");
                break;
        }

        switch (selectedTestResult)
        {
            case TestResult.Pass:
                selectionStringBuilder.Append("Result: Pass.");
                break;
            case TestResult.Fail:
                selectionStringBuilder.Append("Result: Fail.");
                break;
        }

        // Task 4, шаг 5: результат выводится в Content метки testDetails
        testDetails.Content = selectionStringBuilder.ToString();
    }
}